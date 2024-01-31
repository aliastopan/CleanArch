using CleanArch.Domain.Aggregates.Identity;
using CleanArch.Domain.Enums;

namespace CleanArch.Infrastructure.Handlers;

internal sealed class IdentityAggregateHandler : IIdentityAggregateHandler
{
    private readonly AppDbContextFactory _dbContextFactory;
    private readonly IPasswordService _passwordService;
    private readonly IDateTimeService _dateTimeService;

    public IdentityAggregateHandler(AppDbContextFactory dbContextFactory,
        IPasswordService passwordService,
        IDateTimeService dateTimeService)
    {
        _dbContextFactory = dbContextFactory;
        _passwordService = passwordService;
        _dateTimeService = dateTimeService;
    }

    public async Task<UserAccount> CreateUserAccountAsync(string username, string firstName, string lastName,
        DateOnly dateOfBirth, string emailAddress, string password)
    {
        var passwordHash = _passwordService.HashPassword(password, out string passwordSalt);
        var creationDate = _dateTimeService.DateTimeOffsetNow;
        var userAccount = new UserAccount(username, firstName, lastName,
            dateOfBirth, emailAddress, passwordHash, passwordSalt, creationDate);

        using var dbContext = _dbContextFactory.CreateDbContext();

        dbContext.UserAccounts.Add(userAccount);
        await dbContext.SaveChangesAsync();

        return userAccount;
    }

    public async Task SignUserAsync(UserAccount userAccount, RefreshToken refreshToken)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        userAccount.LastSignedIn = _dateTimeService.DateTimeOffsetNow;
        dbContext.UserAccounts.Update(userAccount);
        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync();
    }

    internal async Task<Result<UserAccount>> TryGetUserAccountAsync(Func<AppDbContext, Task<UserAccount?>> getUserAccount)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var userAccount = await getUserAccount(dbContext);
        if (userAccount is null)
        {
            var error = new Error("User not found.", ErrorSeverity.Warning);
            return Result<UserAccount>.NotFound(error);
        }

        return Result<UserAccount>.Ok(userAccount);
    }

    public async Task<Result<UserAccount>> TryGetUserAccountAsync(Guid userAccountId)
    {
        return await TryGetUserAccountAsync(db => db.GetUserAccountByIdAsync(userAccountId));
    }

    public async Task<Result<UserAccount>> TryGetUserAccountAsync(string username)
    {
        return await TryGetUserAccountAsync(db => db.GetUserAccountByUsernameAsync(username));
    }

    public async Task<Result<List<UserAccount>>> TryGetRangeUserAccountsAsync(int range = 0)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var userAccounts = await dbContext.GetRangeUserAccountAsync(range);
        if (userAccounts.Count is 0)
        {
            var error = new Error("Users not found.", ErrorSeverity.Emergency);
            return Result<List<UserAccount>>.NotFound(error);
        }

        return Result<List<UserAccount>>.Ok(userAccounts);
    }

    public async Task<Result> TryValidateAvailabilityAsync(string username, string emailAddress)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var isUsernameAvailable = (await dbContext.GetUserAccountByUsernameAsync(username)) is null;
        var isEmailAvailable = (await dbContext.GetUserAccountByUsernameAsync(username)) is null;

        var errors = Array.Empty<Error>();
        if (!isUsernameAvailable)
        {
            var usernameTaken = new Error("Username is already taken.", ErrorSeverity.Warning);
            errors = [.. errors, usernameTaken];
        }

        if (!isEmailAvailable)
        {
            var emailInUse = new Error("Email address is already in use.", ErrorSeverity.Warning);
            errors = [.. errors, emailInUse];
        }

        if (errors.Length > 0)
        {
            return Result.Conflict(errors);
        }

        return Result.Ok();
    }

    public async Task UpdateUserRoleAsync(UserAccount userAccount, UserRole userRole)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        userAccount.User = userAccount.User.ChangeUserRole(userRole);
        dbContext.UserAccounts.Update(userAccount);

        await dbContext.SaveChangesAsync();
    }

    public async Task GrantPrivilegeAsync(UserAccount userAccount, UserPrivilege userPrivilege)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        userAccount.User = userAccount.User.AddPrivilege(userPrivilege);
        dbContext.UserAccounts.Update(userAccount);

        await dbContext.SaveChangesAsync();
    }

    public async Task RevokePrivilegeAsync(UserAccount userAccount, UserPrivilege userPrivilege)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        userAccount.User = userAccount.User.RemovePrivilege(userPrivilege);
        dbContext.UserAccounts.Update(userAccount);

        await dbContext.SaveChangesAsync();
    }

    public Result TryValidatePassword(string password, string passwordSalt, string passwordHash)
    {
        var isIncorrectPassword = !_passwordService.VerifyPassword(password, passwordSalt, passwordHash);
        if (isIncorrectPassword)
        {
            var error = new Error("Incorrect password.", ErrorSeverity.Warning);
            return Result.Unauthorized(error);
        }

        return Result.Ok();
    }

    public Result TryValidatePassword(string newPassword, string oldPassword, string passwordSalt, string passwordHash)
    {
        var tryValidateAccess = TryValidatePassword(oldPassword, passwordSalt, passwordHash);
        if (tryValidateAccess.IsFailure())
        {
            return Result.Inherit(result: tryValidateAccess);
        }

        var isSamePassword = _passwordService.VerifyPassword(newPassword, passwordSalt, passwordHash);
        if (isSamePassword)
        {
            var error = new Error("New password cannot be the same as the old password.", ErrorSeverity.Warning);
            return Result.Invalid(error);
        }

        return Result.Ok();
    }

    public async Task UpdatePasswordAsync(UserAccount userAccount, string newPassword)
    {
        userAccount.PasswordHash = _passwordService.HashPassword(newPassword, out var passwordSalt);
        userAccount.PasswordSalt = passwordSalt;

        using var dbContext = _dbContextFactory.CreateDbContext();

        dbContext.UserAccounts.Update(userAccount);
        await dbContext.SaveChangesAsync();
    }

    public async Task InvalidateRefreshTokensAsync(UserAccount userAccount)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var refreshTokens = await dbContext.GetRefreshTokensByUserAccountIdAsync(userAccount.UserAccountId);
        foreach (var refreshToken in refreshTokens)
        {
            refreshToken.IsInvalidated = true;
        }

        dbContext.RefreshTokens.UpdateRange(refreshTokens);

        await dbContext.SaveChangesAsync();
    }

    public async Task RotateRefreshTokenAsync(RefreshToken previous, RefreshToken current)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        dbContext.RefreshTokens.Update(previous);
        dbContext.RefreshTokens.Add(current);

        await dbContext.SaveChangesAsync();
    }
}
