using CleanArch.Domain.Aggregates.Identity;
using CleanArch.Domain.Enums;

namespace CleanArch.Infrastructure.Managers;

internal sealed class IdentityManager : IIdentityManager
{
    private readonly IIdentityAggregateService _identityAggregateService;

    public IdentityManager(IIdentityAggregateService identityAggregateService)
    {
        _identityAggregateService = identityAggregateService;
    }

    public async Task<Result<UserAccount>> TrySignUpAsync(string username, string firstName, string lastName,
        DateOnly dateOfBirth, string emailAddress, string password)
    {
        var TryValidateAvailability = await _identityAggregateService.TryValidateAvailabilityAsync(username, emailAddress);
        if (TryValidateAvailability.IsFailure)
        {
            return Result<UserAccount>.Inherit(result: TryValidateAvailability);
        }

        var userAccount = await _identityAggregateService.CreateUserAccountAsync(username, firstName, lastName, dateOfBirth, emailAddress, password);

        return Result<UserAccount>.Ok(userAccount);
    }

    public async Task<Result> TryGrantRoleAsync(Guid userAccountId, string role)
    {
        var tryGetUserAccount = await _identityAggregateService.TryGetUserAccountAsync(userAccountId);
        if (tryGetUserAccount.IsFailure)
        {
            return Result.Inherit(result: tryGetUserAccount);
        }

        var userAccount = tryGetUserAccount.Value;

        var userRole = (UserRole)Enum.Parse(typeof(UserRole), role);
        if (userAccount.UserRoles.Contains(userRole))
        {
            var error = new Error("Cannot have duplicate role.", ErrorSeverity.Warning);
            return Result.Conflict(error);
        }

        await _identityAggregateService.GrantRoleAsync(userAccount, userRole);

        return Result.Ok();
    }

    public async Task<Result> TryRevokeRoleAsync(Guid userAccountId, string role)
    {
        var tryGetUserAccount = await _identityAggregateService.TryGetUserAccountAsync(userAccountId);
        if (tryGetUserAccount.IsFailure)
        {
            return Result.Inherit(result: tryGetUserAccount);
        }

        var userAccount = tryGetUserAccount.Value;
        var userRole = (UserRole)Enum.Parse(typeof(UserRole), role);
        if (!userAccount.UserRoles.Contains(userRole))
        {
            var error = new Error("Role does not exist.", ErrorSeverity.Warning);
            return Result.Invalid(error);
        }

        await _identityAggregateService.RevokeRoleAsync(userAccount, userRole);;

        return Result.Ok();
    }

    public async Task<Result> TryResetPasswordAsync(Guid userAccountId, string oldPassword, string newPassword)
    {
        var tryGetUserAccount = await _identityAggregateService.TryGetUserAccountAsync(userAccountId);
        if (tryGetUserAccount.IsFailure)
        {
            return Result.Inherit(result: tryGetUserAccount);
        }

        var userAccount = tryGetUserAccount.Value;
        var tryValidatePassword = _identityAggregateService.TryValidatePassword(newPassword, oldPassword, userAccount.PasswordSalt, userAccount.PasswordHash);
        if (tryValidatePassword.IsFailure)
        {
            return Result.Inherit(result: tryValidatePassword);
        }

        await _identityAggregateService.UpdatePasswordAsync(userAccount, newPassword);
        await _identityAggregateService.InvalidateRefreshTokensAsync(userAccount);

        return Result.Ok();
    }
}
