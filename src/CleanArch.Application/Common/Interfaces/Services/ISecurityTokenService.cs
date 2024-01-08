using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Application.Common.Interfaces.Services;

[Obsolete("Use 'IAccessTokenService' or 'IRefreshTokenService' instead")]
public interface ISecurityTokenService
{
    string GenerateAccessToken(UserAccount user);
    Result<RefreshToken> TryGenerateRefreshToken(string accessToken, UserAccount user);
    Result<RefreshToken> TryValidateSecurityToken(string accessToken, string refreshTokenStr);
}
