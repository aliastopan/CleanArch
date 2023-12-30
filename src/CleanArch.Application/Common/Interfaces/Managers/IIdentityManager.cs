using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Application.Common.Interfaces.Managers;

public interface IIdentityManager
{
    Task<Result<UserAccount>> TrySignUpAsync(string username, string firstName, string lastName,
        DateOnly dateOfBirth, string emailAddress, string password);
    Task<Result> TryGrantRoleAsync(Guid userAccountId, string role);
    Task<Result> TryRevokeRoleAsync(Guid userAccountId, string role);
    Task<Result> TryResetPasswordAsync(Guid userAccountId, string oldPassword, string newPassword);
}
