using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Application.Common.Interfaces.Services;

public interface IRefreshTokenService
{
    Result<RefreshToken> TryGenerateRefreshToken(string accessToken, UserAccount user);
    Result<RefreshToken> TryValidateSecurityToken(string accessToken, string refreshTokenStr);
}
