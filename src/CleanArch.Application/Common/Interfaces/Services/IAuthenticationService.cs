using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Application.Common.Interfaces.Services;

public interface IAuthenticationService
{
    Task<Result<(string accessToken, RefreshToken refreshToken)>> TrySignInAsync(string username, string password);
    Task<Result<(string accessToken, RefreshToken refreshToken)>> TryRefreshAccessAsync(string accessToken, string refreshToken);
    Task<Result> TryAccessPromptAsync(Guid userAccountId, string password);
}
