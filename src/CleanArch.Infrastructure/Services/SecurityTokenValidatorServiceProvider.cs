using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace CleanArch.Infrastructure.Services;

internal sealed class SecurityTokenValidatorServiceProvider : ISecurityTokenValidatorService
{
    private readonly IConfiguration _configuration;

    public SecurityTokenValidatorServiceProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenValidationParameters GetAccessTokenValidationParameters()
    {
        var apiKey = _configuration[UserSecrets.Element.ApiKey];
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
}
