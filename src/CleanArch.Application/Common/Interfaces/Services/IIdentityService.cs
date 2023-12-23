using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Application.Common.Interfaces.Services;

public interface IIdentityService
{
    Task<Result<UserAccount>> TrySignUpAsync(string username, string firstName, string lastName,
        DateOnly dateOfBirth, string emailAddress, string password);
    Task<Result> TryGrantRoleAsync(Guid userAccountId, string role);
    Task<Result> TryRevokeRoleAsync(Guid userAccountId, string role);
    Task<Result> TryResetPasswordAsync(Guid userAccountId, string oldPassword, string newPassword);

}
