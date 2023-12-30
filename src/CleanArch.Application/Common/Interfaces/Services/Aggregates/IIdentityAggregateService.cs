using CleanArch.Domain.Aggregates.Identity;
using CleanArch.Domain.Enums;

namespace CleanArch.Application.Common.Interfaces.Services.Aggregates;

public interface IIdentityAggregateService
{
    // user account
    Task<UserAccount> CreateUserAccountAsync(string username, string firstName, string lastName,
        DateOnly dateOfBirth, string emailAddress, string password);
    Task SignUserAsync(UserAccount userAccount, RefreshToken refreshToken);
    Task<Result<UserAccount>> TryGetUserAccountAsync(Guid userAccountId);
    Task<Result<UserAccount>> TryGetUserAccountAsync(string username);
    Task<Result> TryValidateAvailabilityAsync(string username, string emailAddress);

    // user role
    Task GrantRoleAsync(UserAccount userAccount, UserRole userRole);
    Task RevokeRoleAsync(UserAccount userAccount, UserRole userRole);

    // password
    Result TryValidatePassword(string password, string passwordSalt, string passwordHash);
    Result TryValidatePassword(string newPassword, string oldPassword, string passwordSalt, string passwordHash);
    Task UpdatePasswordAsync(UserAccount userAccount, string newPassword);

    // refresh token
    Task InvalidateRefreshTokensAsync(UserAccount userAccount);
    Task RotateRefreshTokenAsync(RefreshToken previous, RefreshToken current);
}
