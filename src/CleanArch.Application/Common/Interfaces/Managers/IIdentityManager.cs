using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Application.Common.Interfaces.Managers;

public interface IIdentityManager
{
    Task<Result<UserAccount>> TrySignUpAsync(string username, string firstName, string lastName,
        DateOnly dateOfBirth, string emailAddress, string password);
    Task<Result> TryGrantPrivilegeAsync(Guid userAccountId, string privilege);
    Task<Result> TryRevokePrivilegeAsync(Guid userAccountId, string privilege);
    Task<Result> TryResetPasswordAsync(Guid userAccountId, string oldPassword, string newPassword);
}
