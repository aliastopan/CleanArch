using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Application.Common.Interfaces.Managers;

public interface IAuthenticationManager
{
    Task<Result<(string accessToken, RefreshToken refreshToken)>> TrySignInAsync(string username, string password);
    Task<Result<(string accessToken, RefreshToken refreshToken)>> TryRefreshAccessAsync(string accessToken, string refreshTokenStr);
    Task<Result> TryAccessPromptAsync(Guid userAccountId, string password);
}
