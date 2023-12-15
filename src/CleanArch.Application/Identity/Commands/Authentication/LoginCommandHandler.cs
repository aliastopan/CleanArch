using CleanArch.Domain.Aggregates;
using CleanArch.Domain.Entities.Identity;

namespace CleanArch.Application.Identity.Commands.Authentication;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginCommandResponse>>
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;
    private readonly IPasswordService _passwordService;
    private readonly ISecurityTokenService _securityTokenService;

    public LoginCommandHandler(IAppDbContextFactory<IAppDbContext> dbContextFactory,
        IPasswordService passwordService,
        ISecurityTokenService securityTokenService)
    {
        _dbContextFactory = dbContextFactory;
        _passwordService = passwordService;
        _securityTokenService = securityTokenService;
    }

    public async ValueTask<Result<LoginCommandResponse>> Handle(LoginCommand request,
        CancellationToken cancellationToken)
    {
        Result<LoginCommandResponse> result;

        var isValid = request.TryValidate(out var errors);
        if(!isValid)
        {
            result = Result<LoginCommandResponse>.Invalid(errors);
            return await ValueTask.FromResult(result);
        }

        var userAccount = await SearchUserAccountAsync(request.Username);
        if(userAccount is null)
        {
            var error = new Error("User does not exist.", ErrorSeverity.Warning);
            result = Result<LoginCommandResponse>.NotFound(error);
            return await ValueTask.FromResult(result);
        }

        var validatePassword = ValidatePassword(request.Password, userAccount.PasswordSalt, userAccount.PasswordHash);
        if(!validatePassword.IsSuccess)
        {
            result = Result<LoginCommandResponse>.Inherit(result: validatePassword);
            return await ValueTask.FromResult(result);
        }

        var (accessToken, refreshToken) = await SignUserAsync(userAccount);

        var response = new LoginCommandResponse(userAccount.UserAccountId, accessToken, refreshToken.Token);
        result = Result<LoginCommandResponse>.Ok(response);
        return await ValueTask.FromResult(result);
    }

    private Result ValidatePassword(string password, string passwordSalt, string passwordHash)
    {
        var isVerified = _passwordService.VerifyPassword(password, passwordSalt, passwordHash);
        if(!isVerified)
        {
            var error = new Error("Incorrect password.", ErrorSeverity.Warning);
            return Result.Unauthorized(error);
        }

        return Result.Ok();
    }

    private async Task<UserAccount?> SearchUserAccountAsync(string username)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var user = await dbContext.GetUserAccountByUsernameAsync(username);
        return user!;
    }

    private async Task<(string accessToken, RefreshToken refreshToken)> SignUserAsync(UserAccount userAccount)
    {
        var accessToken = _securityTokenService.GenerateAccessToken(userAccount);
        var refreshToken = _securityTokenService.GenerateRefreshToken(accessToken, userAccount).Value;

        using var dbContext = _dbContextFactory.CreateDbContext();

        userAccount.LastLoggedIn = DateTimeOffset.Now;
        dbContext.UserAccounts.Update(userAccount);
        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync();

        return (accessToken, refreshToken);
    }
}