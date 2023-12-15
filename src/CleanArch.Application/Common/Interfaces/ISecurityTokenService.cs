using CleanArch.Domain.Aggregates;
using CleanArch.Domain.Entities.Identity;

namespace CleanArch.Application.Common.Interfaces;

public interface ISecurityTokenService
{
    string GenerateAccessToken(UserAccount user);
    Result<RefreshToken> GenerateRefreshToken(string accessToken, UserAccount user);
    Result<RefreshToken> ValidateRefreshToken(string accessToken, string refreshToken);
}
