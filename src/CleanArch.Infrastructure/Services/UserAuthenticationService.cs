using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Infrastructure.Services;

internal sealed class UserAuthenticationService : IUserAuthenticationService
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;
    private readonly IPasswordService _passwordService;
    private readonly ISecurityTokenService _securityTokenService;

    public UserAuthenticationService(IAppDbContextFactory<IAppDbContext> dbContextFactory,
        IPasswordService passwordService,
        ISecurityTokenService securityTokenService)
    {
        _dbContextFactory = dbContextFactory;
        _passwordService = passwordService;
        _securityTokenService = securityTokenService;
    }

    public async Task<Result<(string accessToken, RefreshToken refreshToken)>> TryAuthenticateUserAsync(string username, string password)
    {
        var userAccount = await SearchUserAccountAsync(username);
        if(userAccount is null)
        {
            var error = new Error("User does not exist.", ErrorSeverity.Warning);
            return Result<(string, RefreshToken)>.NotFound(error);
        }

        var validatePassword = TryValidatePassword(password, userAccount.PasswordSalt, userAccount.PasswordHash);
        if(!validatePassword.IsSuccess)
        {
            return Result<(string, RefreshToken)>.Inherit(result: validatePassword);
        }

        var access = await SignInAsync(userAccount);
        return Result<(string, RefreshToken)>.Ok(access);
    }

    private async Task<(string accessToken, RefreshToken refreshToken)> SignInAsync(UserAccount userAccount)
    {
        var accessToken = _securityTokenService.GenerateAccessToken(userAccount);
        var refreshToken = _securityTokenService.GenerateRefreshToken(accessToken, userAccount).Value;

        using var dbContext = _dbContextFactory.CreateDbContext();

        userAccount.LastLoggedIn = DateTimeOffset.Now;
        dbContext.UserAccounts.Update(userAccount);
        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync();

        return(accessToken, refreshToken);
    }

    private async Task<UserAccount?> SearchUserAccountAsync(string username)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var user = await dbContext.GetUserAccountByUsernameAsync(username);
        return user!;
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
