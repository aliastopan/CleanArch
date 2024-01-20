using CleanArch.Domain.Aggregates.Identity;
using CleanArch.Domain.Enums;

namespace CleanArch.Infrastructure.Managers;

internal sealed class IdentityManager : IIdentityManager
{
    private readonly IIdentityAggregateHandler _identityAggregateHandler;

    public IdentityManager(IIdentityAggregateHandler identityAggregateHandler)
    {
        _identityAggregateHandler = identityAggregateHandler;
    }

    public async Task<Result<UserAccount>> TrySignUpAsync(string username, string firstName, string lastName,
        DateOnly dateOfBirth, string emailAddress, string password)
    {
        var TryValidateAvailability = await _identityAggregateHandler.TryValidateAvailabilityAsync(username, emailAddress);
        if (TryValidateAvailability.IsFailure())
        {
            return Result<UserAccount>.Inherit(result: TryValidateAvailability);
        }

        var userAccount = await _identityAggregateHandler.CreateUserAccountAsync(username, firstName, lastName, dateOfBirth, emailAddress, password);

        return Result<UserAccount>.Ok(userAccount);
    }


    public async Task<Result> TrySetRoleAsync(Guid userAccountId, string role)
    {
        var tryGetUserAccount = await _identityAggregateHandler.TryGetUserAccountAsync(userAccountId);
        if (tryGetUserAccount.IsFailure())
        {
            return Result.Inherit(result: tryGetUserAccount);
        }

        var userAccount = tryGetUserAccount.Value;
        var userRole = (UserRole)Enum.Parse(typeof(UserRole), role);

        await _identityAggregateHandler.UpdateUserRoleAsync(userAccount, userRole);

        return Result.Ok();
    }

    public async Task<Result> TryGrantPrivilegeAsync(Guid userAccountId, string privilege)
    {
        var tryGetUserAccount = await _identityAggregateHandler.TryGetUserAccountAsync(userAccountId);
        if (tryGetUserAccount.IsFailure())
        {
            return Result.Inherit(result: tryGetUserAccount);
        }

        var userAccount = tryGetUserAccount.Value;

        // TODO: add redundancy parsing guard to return error
        var userPrivilege = (UserPrivilege)Enum.Parse(typeof(UserPrivilege), privilege);
        var hasDuplicatePrivilege = userAccount.User.UserPrivileges.Contains(userPrivilege);
        if (hasDuplicatePrivilege)
        {
            var error = new Error("Cannot have duplicate privilege.", ErrorSeverity.Warning);
            return Result.Conflict(error);
        }

        await _identityAggregateHandler.GrantPrivilegeAsync(userAccount, userPrivilege);

        return Result.Ok();
    }

    public async Task<Result> TryRevokePrivilegeAsync(Guid userAccountId, string privilege)
    {
        var tryGetUserAccount = await _identityAggregateHandler.TryGetUserAccountAsync(userAccountId);
        if (tryGetUserAccount.IsFailure())
        {
            return Result.Inherit(result: tryGetUserAccount);
        }

        var userAccount = tryGetUserAccount.Value;
        var userPrivilege = (UserPrivilege)Enum.Parse(typeof(UserPrivilege), privilege);
        var hasMissingPrivilege = !userAccount.User.UserPrivileges.Contains(userPrivilege);
        if (hasMissingPrivilege)
        {
            var error = new Error("Privilege does not exist.", ErrorSeverity.Warning);
            return Result.Invalid(error);
        }

        await _identityAggregateHandler.RevokePrivilegeAsync(userAccount, userPrivilege);;

        return Result.Ok();
    }

    public async Task<Result> TryResetPasswordAsync(Guid userAccountId, string oldPassword, string newPassword)
    {
        var tryGetUserAccount = await _identityAggregateHandler.TryGetUserAccountAsync(userAccountId);
        if (tryGetUserAccount.IsFailure())
        {
            return Result.Inherit(result: tryGetUserAccount);
        }

        var userAccount = tryGetUserAccount.Value;
        var tryValidatePassword = _identityAggregateHandler.TryValidatePassword(newPassword, oldPassword, userAccount.PasswordSalt, userAccount.PasswordHash);
        if (tryValidatePassword.IsFailure())
        {
            return Result.Inherit(result: tryValidatePassword);
        }

        await _identityAggregateHandler.UpdatePasswordAsync(userAccount, newPassword);
        await _identityAggregateHandler.InvalidateRefreshTokensAsync(userAccount);

        return Result.Ok();
    }
}
