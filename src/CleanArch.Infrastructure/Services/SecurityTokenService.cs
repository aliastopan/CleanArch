using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using CleanArch.Domain.Entities.Identity;

namespace CleanArch.Infrastructure.Services;

internal sealed class SecurityTokenService : ISecurityTokenService
{
    private readonly UserSecrets _userSecrets;
    private readonly SecurityTokenSettings _securityTokenSettings;

    public SecurityTokenService(IOptions<UserSecrets> userSecrets,
        IOptions<SecurityTokenSettings> securityTokenSettings)
    {
        _userSecrets = userSecrets.Value;
        _securityTokenSettings = securityTokenSettings.Value;
    }

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_userSecrets.ApiKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtClaimTypes.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtClaimTypes.Sub, user.UserId.ToString()),
            new Claim(JwtClaimTypes.UniqueName, user.Username),
            new Claim(JwtClaimTypes.Role, user.Role.ToString())
        };

        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwt = new JwtSecurityToken(
            issuer: _securityTokenSettings.Issuer,
            audience: _securityTokenSettings.Audience,
            expires: DateTime.UtcNow.Add(_securityTokenSettings.AccessTokenLifeTime),
            claims: claims,
            signingCredentials: signingCredentials);

        return jwtHandler.WriteToken(jwt);
    }
}
