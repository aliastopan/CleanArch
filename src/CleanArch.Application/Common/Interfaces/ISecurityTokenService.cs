using CleanArch.Domain.Entities.Identity;

namespace CleanArch.Application.Common.Interfaces;

public interface ISecurityTokenService
{
    string GenerateAccessToken(User user);
    Result<RefreshToken> GenerateRefreshToken(string accessToken, User user);
    Result<RefreshToken> ValidateRefreshToken(string accessToken, string refreshToken);
}
