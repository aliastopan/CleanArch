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

        var user = await SearchUserAsync(request.Username);
        if(user is null)
        {
            var error = new Error("User is not registered", ErrorSeverity.Warning);
            result = Result<LoginCommandResponse>.NotFound(error);
            return await ValueTask.FromResult(result);
        }

        var validatePassword = ValidatePassword(request.Password, user.PasswordSalt, user.PasswordHash);
        if(!validatePassword.IsSuccess)
        {
            result = Result<LoginCommandResponse>.Inherit(result: validatePassword);
            return await ValueTask.FromResult(result);
        }

        var accessToken = _securityTokenService.GenerateAccessToken(user);
        var refreshToken = _securityTokenService.GenerateRefreshToken(accessToken, user).Value;

        using var dbContext = _dbContextFactory.CreateDbContext();

        user.LastLoggedIn = DateTimeOffset.Now;
        dbContext.Users.Update(user);
        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync();

        var response = new LoginCommandResponse(user.UserId, accessToken, refreshToken.Token);
        result = Result<LoginCommandResponse>.Ok(response);
        return await ValueTask.FromResult(result);
    }

    private Result ValidatePassword(string password, string passwordSalt, string passwordHash)
    {
        var isValid = _passwordService.VerifyPassword(password, passwordSalt, passwordHash);
        if(!isValid)
        {
            var error = new Error("Incorrect password.", ErrorSeverity.Warning);
            return Result.Unauthorized(error);
        }

        return Result.Ok();
    }

    private async Task<User?> SearchUserAsync(string username)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);
        return user;
    }
}