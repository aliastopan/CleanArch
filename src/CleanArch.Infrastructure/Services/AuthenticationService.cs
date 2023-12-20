using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Infrastructure.Services;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;
    private readonly IPasswordService _passwordService;
    private readonly ISecurityTokenService _securityTokenService;
    private readonly IDateTimeService _dateTimeService;

    public AuthenticationService(IAppDbContextFactory<IAppDbContext> dbContextFactory,
        IPasswordService passwordService,
        ISecurityTokenService securityTokenService,
        IDateTimeService dateTimeService)
    {
        _dbContextFactory = dbContextFactory;
        _passwordService = passwordService;
        _securityTokenService = securityTokenService;
        _dateTimeService = dateTimeService;
    }

    public async Task<Result<(string accessToken, RefreshToken refreshToken)>> TrySignInAsync(string username, string password)
    {
        var tryGetUserAccount = await TryGetUserAccountAsync(username);
        if(!tryGetUserAccount.IsSuccess)
        {
            return Result<(string, RefreshToken)>.Inherit(result: tryGetUserAccount);
        }

        var userAccount = tryGetUserAccount.Value;

        var validatePassword = TryValidatePassword(password, userAccount.PasswordSalt, userAccount.PasswordHash);
        if(!validatePassword.IsSuccess)
        {
            return Result<(string, RefreshToken)>.Inherit(result: validatePassword);
        }

        // sign-in protocol
        var accessToken = _securityTokenService.GenerateAccessToken(userAccount);
        var tryGetRefreshToken = _securityTokenService.TryGenerateRefreshToken(accessToken, userAccount);
        if(!tryGetRefreshToken.IsSuccess)
        {
            return Result<(string, RefreshToken)>.Inherit(result: tryGetRefreshToken);
        }

        var refreshToken = tryGetRefreshToken.Value;

        using var dbContext = _dbContextFactory.CreateDbContext();

        userAccount.LastLoggedIn = _dateTimeService.UtcNow;
        dbContext.UserAccounts.Update(userAccount);
        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync();

        return Result<(string, RefreshToken)>.Ok((accessToken, refreshToken));
    }

    public async Task<Result<(string accessToken, RefreshToken refreshToken)>> TryRefreshAccessAsync(string accessToken, string refreshToken)
    {
        var tryValidateSecurityToken = _securityTokenService.TryValidateSecurityToken(accessToken, refreshToken);
        if(!tryValidateSecurityToken.IsSuccess)
        {
            return Result<(string, RefreshToken)>.Inherit(result: tryValidateSecurityToken);
        }

        var userAccount = tryValidateSecurityToken.Value.UserAccount;
        accessToken = _securityTokenService.GenerateAccessToken(userAccount);
        var tryGetRefreshToken = _securityTokenService.TryGenerateRefreshToken(accessToken, userAccount);
        if(!tryGetRefreshToken.IsSuccess)
        {
            return Result<(string, RefreshToken)>.Inherit(result: tryGetRefreshToken);
        }

        RefreshToken previous = tryValidateSecurityToken.Value;
        RefreshToken current = tryGetRefreshToken.Value;

        using var dbContext = _dbContextFactory.CreateDbContext();

        dbContext.RefreshTokens.Update(previous);
        dbContext.RefreshTokens.Add(current);
        await dbContext.SaveChangesAsync();

        return Result<(string, RefreshToken)>.Ok((accessToken, current));
    }

    public async Task<Result> TryAccessPromptAsync(Guid userAccountId, string password)
    {
        var tryGetUserAccount = await TryGetUserAccountAsync(userAccountId);
        if(!tryGetUserAccount.IsSuccess)
        {
            return Result.Inherit(result: tryGetUserAccount);
        }

        var userAccount = tryGetUserAccount.Value;

        var validatePassword = TryValidatePassword(password, userAccount.PasswordSalt, userAccount.PasswordHash);
        if(!validatePassword.IsSuccess)
        {
            return Result.Inherit(result: validatePassword);
        }

        return Result.Ok();
    }

    private async Task<Result<UserAccount>> TryGetUserAccountAsync(Func<IAppDbContext, Task<UserAccount?>> getUserAccount)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var userAccount = await getUserAccount(dbContext);
        if(userAccount is null)
        {
            var error = new Error("User does not exist.", ErrorSeverity.Warning);
            return Result<UserAccount>.NotFound(error);
        }

        return Result<UserAccount>.Ok(userAccount);
    }

    private async Task<Result<UserAccount>> TryGetUserAccountAsync(Guid userAccountId)
    {
        return await TryGetUserAccountAsync(db => db.GetUserAccountByIdAsync(userAccountId));
    }

    private async Task<Result<UserAccount>> TryGetUserAccountAsync(string username)
    {
        return await TryGetUserAccountAsync(db => db.GetUserAccountByUsernameAsync(username));
    }

    private Result TryValidatePassword(string password, string passwordSalt, string passwordHash)
    {
        var isVerified = _passwordService.VerifyPassword(password, passwordSalt, passwordHash);
        if(!isVerified)
        {
            var error = new Error("Incorrect password.", ErrorSeverity.Warning);
            return Result.Unauthorized(error);
        }

        return Result.Ok();
    }
}
