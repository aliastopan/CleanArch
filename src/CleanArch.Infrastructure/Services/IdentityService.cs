using CleanArch.Domain.Aggregates.Identity;
using CleanArch.Domain.Enums;

namespace CleanArch.Infrastructure.Services;

internal sealed class IdentityService : IIdentityService
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;
    private readonly IPasswordService _passwordService;
    private readonly IDateTimeService _dateTimeService;

    public IdentityService(IAppDbContextFactory<IAppDbContext> dbContextFactory,
        IPasswordService passwordService,
        IDateTimeService dateTimeService)
    {
        _dbContextFactory = dbContextFactory;
        _passwordService = passwordService;
        _dateTimeService = dateTimeService;
    }

    public async Task<Result<UserAccount>> TrySignUpAsync(string username, string email, string password)
    {
        var TryValidateAvailability = await TryValidateAvailabilityAsync(username, email);
        if(!TryValidateAvailability.IsSuccess)
        {
            return Result<UserAccount>.Inherit(result: TryValidateAvailability);
        }

        var userAccount = await CreateUserAccountAsync(username, email, password);
        return Result<UserAccount>.Ok(userAccount);
    }

    public async Task<Result> TryGrantRoleAsync(Guid userAccountId, string role)
    {
        var tryGetUserAccount = await TryGetUserAccountAsync(userAccountId);
        if(!tryGetUserAccount.IsSuccess)
        {
            return Result.Inherit(result: tryGetUserAccount);
        }

        var userAccount = tryGetUserAccount.Value;

        var userRole = (UserRole)Enum.Parse(typeof(UserRole), role);
        if(userAccount.UserRoles.Contains(userRole))
        {
            var error = new Error("Cannot have duplicate role.", ErrorSeverity.Warning);
            return Result.Conflict(error);
        }

        using var dbContext = _dbContextFactory.CreateDbContext();

        userAccount.UserRoles.Add(userRole);
        dbContext.UserAccounts.Update(userAccount);
        await dbContext.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> TryRevokeRoleAsync(Guid userAccountId, string role)
    {
        var tryGetUserAccount = await TryGetUserAccountAsync(userAccountId);
        if(!tryGetUserAccount.IsSuccess)
        {
            return Result.Inherit(result: tryGetUserAccount);
        }

        var userAccount = tryGetUserAccount.Value;
        var userRole = (UserRole)Enum.Parse(typeof(UserRole), role);
        if(!userAccount.UserRoles.Contains(userRole))
        {
            var error = new Error("Role does not exist.", ErrorSeverity.Warning);
            return Result.Conflict(error);
        }

        using var dbContext = _dbContextFactory.CreateDbContext();

        userAccount.UserRoles.Remove(userRole);
        dbContext.UserAccounts.Update(userAccount);
        await dbContext.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> TryResetPasswordAsync(Guid userAccountId, string oldPassword, string newPassword)
    {
        var tryGetUserAccount = await TryGetUserAccountAsync(userAccountId);
        if(!tryGetUserAccount.IsSuccess)
        {
            return Result.Inherit(result: tryGetUserAccount);
        }

        var userAccount = tryGetUserAccount.Value;
        var tryValidatePassword = TryValidatePassword(newPassword, oldPassword, userAccount.PasswordSalt, userAccount.PasswordHash);
        if(!tryValidatePassword.IsSuccess)
        {
            return Result.Inherit(result: tryValidatePassword);
        }

        await UpdatePassword(userAccount, newPassword);
        await InvalidateRefreshToken(userAccount);

        return Result.Ok();
    }

    private async Task<UserAccount> CreateUserAccountAsync(string username, string email, string password)
    {
        var hash = _passwordService.HashPassword(password, out string salt);
        var creationDate = _dateTimeService.DateTimeOffsetNow;
        var userAccount = new UserAccount(username, email, hash, salt, creationDate);

        using var dbContext = _dbContextFactory.CreateDbContext();

        dbContext.UserAccounts.Add(userAccount);
        await dbContext.SaveChangesAsync();

        return userAccount;
    }

    private async Task UpdatePassword(UserAccount userAccount, string newPassword)
    {
        userAccount.PasswordHash = _passwordService.HashPassword(newPassword, out var salt);
        userAccount.PasswordSalt = salt;

        using var dbContext = _dbContextFactory.CreateDbContext();

        dbContext.UserAccounts.Update(userAccount);
        await dbContext.SaveChangesAsync();
    }

    private Result TryValidatePassword(string newPassword, string oldPassword, string passwordSalt, string passwordHash)
    {
        var isVerified = _passwordService.VerifyPassword(oldPassword, passwordSalt, passwordHash);
        if(!isVerified)
        {
            var error = new Error("Incorrect password.", ErrorSeverity.Warning);
            return Result.Unauthorized(error);
        }

        var isNew = !_passwordService.VerifyPassword(newPassword, passwordSalt, passwordHash);
        if(!isNew)
        {
            var error = new Error("New password cannot be the same as the old password.", ErrorSeverity.Warning);
            return Result.Invalid(error);
        }

        return Result.Ok();
    }

    private async Task<Result<UserAccount>> TryGetUserAccountAsync(Guid userAccountId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var userAccount = await dbContext.GetUserAccountByIdAsync(userAccountId);
        if(userAccount is null)
        {
            var error = new Error("User not found.", ErrorSeverity.Warning);
            return Result<UserAccount>.NotFound(error);
        }

        return Result<UserAccount>.Ok(userAccount);
    }

    private async Task<Result> TryValidateAvailabilityAsync(string username, string email)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var userAccount = await dbContext.GetUserAccountByUsernameAsync(username);
        if(userAccount is not null)
        {
            var error = new Error("Username is already taken.", ErrorSeverity.Warning);
            return Result.Conflict(error);
        }

        userAccount = await dbContext.GetUserAccountByEmailAsync(email);
        if(userAccount is not null)
        {
            var error = new Error("Email is already in use.", ErrorSeverity.Warning);
            return Result.Conflict(error);
        }

        return Result.Ok();
    }

    private async Task InvalidateRefreshToken(UserAccount userAccount)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var refreshTokens = await dbContext.GetRefreshTokensByUserAccountIdAsync(userAccount.UserAccountId);
        refreshTokens.ForEach(x => x.IsInvalidated = true);
        dbContext.RefreshTokens.UpdateRange(refreshTokens);

        await dbContext.SaveChangesAsync();
    }
}
