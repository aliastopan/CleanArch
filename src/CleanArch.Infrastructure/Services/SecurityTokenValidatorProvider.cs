using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace CleanArch.Infrastructure.Services;

internal sealed class SecurityTokenValidatorProvider : ISecurityTokenValidatorService
{
    private readonly AppSecretSettings _appSecretSettings;
    private readonly SecurityTokenSettings _securityTokenSettings;

    public SecurityTokenValidatorProvider(IOptions<AppSecretSettings> appSecretSettings,
        IOptions<SecurityTokenSettings> securityTokenSettings)
    {
        _appSecretSettings = appSecretSettings.Value;
        _securityTokenSettings = securityTokenSettings.Value;
    }

    public TokenValidationParameters GetAccessTokenValidationParameters()
    {
        var masterKey = _appSecretSettings.MasterKey;
        return new TokenValidationParameters
        {
            ValidIssuer = _securityTokenSettings.Issuer,
            ValidAudience = _securityTokenSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(masterKey!)),
            ValidateIssuer = true,
            ValidateAudience = true,
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    public TokenValidationParameters GetRefreshTokenValidationParameters()
    {
        var masterKey = _appSecretSettings.MasterKey;
        return new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(masterKey!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
        };
    }
}
