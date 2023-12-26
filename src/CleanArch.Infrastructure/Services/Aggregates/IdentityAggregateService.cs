using CleanArch.Domain.Aggregates.Identity;
using CleanArch.Domain.Enums;

namespace CleanArch.Infrastructure.Services.Aggregates;

internal sealed class IdentityAggregateService : IIdentityAggregateService
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;
    private readonly IPasswordService _passwordService;
    private readonly IDateTimeService _dateTimeService;

    public IdentityAggregateService(IAppDbContextFactory<IAppDbContext> dbContextFactory,
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

    internal async Task<Result<UserAccount>> TryGetUserAccountAsync(Func<IAppDbContext, Task<UserAccount?>> getUserAccount)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var userAccount = await getUserAccount(dbContext);
        if(userAccount is null)
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

    public async Task<Result> TryValidateAvailabilityAsync(string username, string emailAddress)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var isUsernameAvailable = (await dbContext.GetUserAccountByUsernameAsync(username)) is null;
        var isEmailAvailable = (await dbContext.GetUserAccountByUsernameAsync(username)) is null;

        var errors = Array.Empty<Error>();
        if(!isUsernameAvailable)
        {
            var usernameTaken = new Error("Username is already taken.", ErrorSeverity.Warning);
            errors = [.. errors, usernameTaken];
        }

        if(!isEmailAvailable)
        {
            var emailInUse = new Error("Email address is already in use.", ErrorSeverity.Warning);
            errors = [.. errors, emailInUse];
        }

        if(errors.Length > 0)
        {
            return Result.Conflict(errors);
        }

        return Result.Ok();
    }

    internal async Task UpdateRoleAsync(Func<UserAccount> updateUserRole)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var userAccount = updateUserRole.Invoke();
        dbContext.UserAccounts.Update(userAccount);

        await dbContext.SaveChangesAsync();
    }

    public async Task GrantRoleAsync(UserAccount userAccount, UserRole userRole)
    {
        await UpdateRoleAsync(() =>
        {
            userAccount.UserRoles.Add(userRole);
            return userAccount;
        });
    }

    public async Task RevokeRoleAsync(UserAccount userAccount, UserRole userRole)
    {
        await UpdateRoleAsync(() =>
        {
            userAccount.UserRoles.Remove(userRole);
            return userAccount;
        });
    }

    public Result TryValidatePassword(string newPassword, string oldPassword, string passwordSalt, string passwordHash)
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
        foreach(var refreshToken in refreshTokens)
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
