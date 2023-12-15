using Microsoft.IdentityModel.Tokens;

namespace CleanArch.Application.Common.Interfaces.Services;

public interface ISecurityTokenValidatorService
{
    TokenValidationParameters GetAccessTokenValidationParameters();
    TokenValidationParameters GetRefreshTokenValidationParameters();
}
