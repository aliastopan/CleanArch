using Microsoft.IdentityModel.Tokens;

namespace CleanArch.Application.Common.Interfaces;

public interface ISecurityTokenValidatorService
{
    TokenValidationParameters GetAccessTokenValidationParameters();
    TokenValidationParameters GetRefreshTokenValidationParameters();
}
