using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using CleanArch.Domain.Aggregates.Identity;

[assembly: InternalsVisibleTo("CleanArch.Tests")]
namespace CleanArch.Infrastructure.Services;

[Obsolete("Use 'IAccessTokenService' or 'IRefreshTokenService' instead")]
internal sealed class SecurityTokenProvider : ISecurityTokenService
{
    private readonly AppDbContextFactory _dbContextFactory;
    private readonly IDateTimeService _dateTimeService;
    private readonly ISecurityTokenValidatorService _securityTokenValidatorService;
    private readonly AppSecretSettings _appSecretSettings;
    private readonly SecurityTokenSettings _securityTokenSettings;

    public SecurityTokenProvider(AppDbContextFactory dbContextFactory,
        IDateTimeService dateTimeService,
        ISecurityTokenValidatorService securityTokenValidatorService,
        IOptions<AppSecretSettings> appSecretSettings,
        IOptions<SecurityTokenSettings> securityTokenSettings)
    {
        _dbContextFactory = dbContextFactory;
        _dateTimeService = dateTimeService;
        _securityTokenValidatorService = securityTokenValidatorService;
        _appSecretSettings = appSecretSettings.Value;
        _securityTokenSettings = securityTokenSettings.Value;
    }

    public string GenerateAccessToken(UserAccount userAccount)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSecretSettings.MasterKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwt = new JwtSecurityToken
        (
            issuer: _securityTokenSettings.Issuer,
            audience: _securityTokenSettings.Audience,
            expires: _dateTimeService.UtcNow.Add(_securityTokenSettings.AccessTokenLifeTime),
            claims: CreateClaims(userAccount),
            signingCredentials: signingCredentials
        );

        return jwtHandler.WriteToken(jwt);
    }

    public Result<RefreshToken> TryGenerateRefreshToken(string accessToken, UserAccount userAccount)
    {
        var principal = GetPrincipalFromToken(accessToken);
        if (principal is null)
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
            CreationDate = _dateTimeService.DateTimeOffsetNow,
            ExpiryDate = _dateTimeService.DateTimeOffsetNow.Add(_securityTokenSettings.RefreshTokenLifeTime),
            FkUserAccountId = userAccount.UserAccountId,
            UserAccount = userAccount,
        };

        return Result<RefreshToken>.Ok(refreshToken);
    }

    public Result<RefreshToken> TryValidateSecurityToken(string accessToken, string refreshTokenStr)
    {
        var principal = GetPrincipalFromToken(accessToken);
        if (principal is null)
        {
            var error = new Error("Refresh token has invalid null principal.", ErrorSeverity.Warning);
            return Result<RefreshToken>.Unauthorized(error);
        }

        RefreshToken? currentRefreshToken;
        using (var dbContext = _dbContextFactory.CreateDbContext())
        {
            currentRefreshToken = dbContext.GetRefreshToken(token: refreshTokenStr);
        }

        if (currentRefreshToken is null)
        {
            var error = new Error("Refresh token not found.", ErrorSeverity.Warning);
            return Result<RefreshToken>.NotFound(error);
        }

        //TODO: validate every condition and stack the error result as array

        if (currentRefreshToken.ExpiryDate < _dateTimeService.UtcNow)
        {
            var error = new Error("Refresh token was expired.", ErrorSeverity.Warning);
            return Result<RefreshToken>.Unauthorized(error);
        }

        if (currentRefreshToken.IsInvalidated)
        {
            var error = new Error("Refresh token was invalidated.", ErrorSeverity.Warning);
            return Result<RefreshToken>.Invalid(error);
        }

        if (currentRefreshToken.IsUsed)
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
            if (!HasValidSecurityAlgorithm(securityToken))
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

    private static List<Claim> CreateClaims(UserAccount userAccount)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtClaimTypes.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtClaimTypes.Sub, userAccount.UserAccountId.ToString()),
            new Claim(JwtClaimTypes.UniqueName, userAccount.User.Username),
            new Claim(JwtClaimTypes.IsVerified, userAccount.IsVerified ? "true" : "false")
        };

        foreach (var privilege in userAccount.User.UserPrivileges)
        {
            claims.Add(new Claim(JwtClaimTypes.Privileges, privilege.ToString()));
        }

        return claims;
    }
}
