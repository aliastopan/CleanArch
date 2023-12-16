using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Infrastructure.Services;

internal sealed class SecurityTokenService : ISecurityTokenService
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;
    private readonly IDateTimeService _dateTimeService;
    private readonly ISecurityTokenValidatorService _securityTokenValidatorService;
    private readonly UserSecrets _userSecrets;
    private readonly SecurityTokenSettings _securityTokenSettings;

    public SecurityTokenService(IAppDbContextFactory<IAppDbContext> dbContextFactory,
        IDateTimeService dateTimeService,
        ISecurityTokenValidatorService securityTokenValidatorService,
        IOptions<UserSecrets> userSecrets,
        IOptions<SecurityTokenSettings> securityTokenSettings)
    {
        _dbContextFactory = dbContextFactory;
        _dateTimeService = dateTimeService;
        _securityTokenValidatorService = securityTokenValidatorService;
        _userSecrets = userSecrets.Value;
        _securityTokenSettings = securityTokenSettings.Value;
    }

    public string GenerateAccessToken(UserAccount userAccount)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_userSecrets.ApiKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtClaimTypes.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtClaimTypes.Sub, userAccount.UserAccountId.ToString()),
            new Claim(JwtClaimTypes.UniqueName, userAccount.User.Username),
            new Claim(JwtClaimTypes.Role, userAccount.UserRole.ToString()),
            new Claim(JwtClaimTypes.IsVerified, userAccount.IsVerified ? "true" : "false")
        };

        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwt = new JwtSecurityToken(
            issuer: _securityTokenSettings.Issuer,
            audience: _securityTokenSettings.Audience,
            expires: _dateTimeService.UtcNow.Add(_securityTokenSettings.AccessTokenLifeTime),
            claims: claims,
            signingCredentials: signingCredentials);

        return jwtHandler.WriteToken(jwt);
    }

    public Result<RefreshToken> TryGenerateRefreshToken(string accessToken, UserAccount userAccount)
    {
        var principal = GetPrincipalFromToken(accessToken);
        if(principal is null)
        {
            var error = new Error("Refresh token has invalid null principal.", ErrorSeverity.Warning);
            return Result<RefreshToken>.Unauthorized(error);
        }

        var jti = principal.Claims.Single(x => x.Type == JwtClaimTypes.Jti).Value;
        var refreshToken = new RefreshToken
        {
            RefreshTokenId = Guid.NewGuid(),
            Token = Guid.NewGuid().ToString(),
            Jti = jti,
            CreationDate = _dateTimeService.UtcNow,
            ExpiryDate = _dateTimeService.UtcNow.Add(_securityTokenSettings.RefreshTokenLifeTime),
            UserAccountId = userAccount.UserAccountId,
            UserAccount = userAccount,
        };

        return Result<RefreshToken>.Ok(refreshToken);
    }

    public Result<RefreshToken> TryValidateRefreshToken(string accessToken, string refreshToken)
    {
        var principal = GetPrincipalFromToken(accessToken);
        if(principal is null)
        {
            var error = new Error("Refresh token has invalid null principal.", ErrorSeverity.Warning);
            return Result<RefreshToken>.Unauthorized(error);
        }

        using var dbContext = _dbContextFactory.CreateDbContext();

        var currentRefreshToken = dbContext.RefreshTokens
            .Include(x => x.UserAccount)
            .SingleOrDefault(x => x.Token == refreshToken);

        if(currentRefreshToken is null)
        {
            var error = new Error("Refresh token not found.", ErrorSeverity.Warning);
            return Result<RefreshToken>.NotFound(error);
        }

        if(currentRefreshToken.ExpiryDate < _dateTimeService.UtcNow)
        {
            var error = new Error("Refresh token was expired.", ErrorSeverity.Warning);
            return Result<RefreshToken>.Unauthorized(error);
        }

        if(currentRefreshToken.IsInvalidated)
        {
            var error = new Error("Refresh token was invalidated.", ErrorSeverity.Warning);
            return Result<RefreshToken>.Invalid(error);
        }

        if(currentRefreshToken.IsUsed)
        {
            var error = new Error("Refresh token was used.", ErrorSeverity.Warning);
            return Result<RefreshToken>.Error(error);
        }

        currentRefreshToken.IsUsed = true;
        return Result<RefreshToken>.Ok(currentRefreshToken);
    }

    private ClaimsPrincipal GetPrincipalFromToken(string accessToken)
    {
        try
        {
            var validationParameters = _securityTokenValidatorService.GetRefreshTokenValidationParameters();
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out var securityToken);
            if(!HasValidSecurityAlgorithm(securityToken))
            {
                return null!;
            }

            return principal;
        }
        catch
        {
            return null!;
        }
    }

    private static bool HasValidSecurityAlgorithm(SecurityToken securityToken)
    {
        var securityAlgorithm = SecurityAlgorithms.HmacSha256;
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        return jwtSecurityToken is not null && jwtSecurityToken!.Header.Alg
            .Equals(securityAlgorithm, StringComparison.InvariantCulture);
    }
}
