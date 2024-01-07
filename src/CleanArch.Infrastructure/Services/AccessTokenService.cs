using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Infrastructure.Services;

internal sealed class AccessTokenService : IAccessTokenService
{
    private readonly IDateTimeService _dateTimeService;
    private readonly ISecurityTokenValidatorService _securityTokenValidatorService;
    private readonly InfrastructureSecretSettings _infrastructureSecretSettings;
    private readonly SecurityTokenSettings _securityTokenSettings;

    public AccessTokenService(IDateTimeService dateTimeService,
        ISecurityTokenValidatorService securityTokenValidatorService,
        IOptions<InfrastructureSecretSettings> infrastructureSecretSettings,
        IOptions<SecurityTokenSettings> securityTokenSettings)
    {
        _dateTimeService = dateTimeService;
        _securityTokenValidatorService = securityTokenValidatorService;
        _infrastructureSecretSettings = infrastructureSecretSettings.Value;
        _securityTokenSettings = securityTokenSettings.Value;
    }

    public string GenerateAccessToken(UserAccount userAccount)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_infrastructureSecretSettings.MasterKey));
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

    public bool ValidateAccessToken(string accessToken)
    {
        try
        {
            var validationParameters = _securityTokenValidatorService.GetAccessTokenValidationParameters();
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out var securityToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public ClaimsPrincipal GetPrincipalFromToken(string accessToken)
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

        foreach (var role in userAccount.UserRoles)
        {
            claims.Add(new Claim(JwtClaimTypes.Roles, role.ToString()));
        }

        return claims;
    }
}
