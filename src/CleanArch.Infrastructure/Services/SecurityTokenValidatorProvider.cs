using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace CleanArch.Infrastructure.Services;

internal sealed class SecurityTokenValidatorProvider : ISecurityTokenValidatorService
{
    private readonly IConfiguration _configuration;

    public SecurityTokenValidatorProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenValidationParameters GetAccessTokenValidationParameters()
    {
        var masterKey = _configuration[AppSecretSettings.Element.MasterKey];
        return new TokenValidationParameters
        {
            ValidIssuer = _configuration[SecurityTokenSettings.Element.Issuer],
            ValidAudience = _configuration[SecurityTokenSettings.Element.Audience],
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
        var masterKey = _configuration[AppSecretSettings.Element.MasterKey];
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
