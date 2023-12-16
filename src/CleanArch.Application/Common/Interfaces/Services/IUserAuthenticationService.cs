using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Application.Common.Interfaces.Services;

public interface IUserAuthenticationService
{
    Task<Result<(string accessToken, RefreshToken refreshToken)>> AuthenticateUserAsync(string username, string password);
}
