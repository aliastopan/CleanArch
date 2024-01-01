using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace CleanArch.Infrastructure.Services;

internal sealed class SecurityTokenValidatorService : ISecurityTokenValidatorService
{
    private readonly IConfiguration _configuration;

    public SecurityTokenValidatorService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenValidationParameters GetAccessTokenValidationParameters()
    {
        var apiKey = _configuration[UserSecretSettings.Element.ApiKey];
        return new TokenValidationParameters
        {
            ValidIssuer = _configuration[SecurityTokenSettings.Element.Issuer],
            ValidAudience = _configuration[SecurityTokenSettings.Element.Issuer],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiKey!)),
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
        var apiKey = _configuration[UserSecretSettings.Element.ApiKey];
        return new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiKey!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
        };
    }
}
