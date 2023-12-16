using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Application.Common.Interfaces.Services;

public interface ISecurityTokenService
{
    string GenerateAccessToken(UserAccount user);
    Result<RefreshToken> TryGenerateRefreshToken(string accessToken, UserAccount user);
    Result<RefreshToken> TryValidateRefreshToken(string accessToken, string refreshToken);
}
