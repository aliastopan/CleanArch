using CleanArch.Domain.Aggregates;

namespace CleanArch.Application.Identity.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;
    private readonly IPasswordService _passwordService;

    public ResetPasswordCommandHandler(IAppDbContextFactory<IAppDbContext> dbContextFactory,
        IPasswordService passwordService)
    {
        _dbContextFactory = dbContextFactory;
        _passwordService = passwordService;
    }

    public async ValueTask<Result> Handle(ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        Result result;
        var isValid = request.TryValidate(out var errors);
        if(!isValid)
        {
            result = Result.Invalid(errors);
            return await ValueTask.FromResult(result);
        }

        using var dbContext = _dbContextFactory.CreateDbContext();

        var userAccount = await dbContext.GetUserAccountByIdAsync(request.UserId);
        if(userAccount is null)
        {
            var error = new Error("User does not exist.", ErrorSeverity.Warning);
            result = Result.NotFound(errors);
            return await ValueTask.FromResult(result);
        }

        var validatePassword = ValidatePassword(request.NewPassword,
            request.OldPassword,
            userAccount.PasswordSalt,
            userAccount.PasswordHash);
        if(!validatePassword.IsSuccess)
        {
            result = Result.Inherit(result: validatePassword);
            return await ValueTask.FromResult(result);
        }

        await ResetPassword(userAccount, request.NewPassword);
        await InvalidateRefreshToken(userAccount);

        result = Result.Ok();
        return await ValueTask.FromResult(result);
    }

    private Result ValidatePassword(string newPassword, string oldPassword, string passwordSalt, string passwordHash)
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

    private async Task ResetPassword(UserAccount userAccount, string newPassword)
    {
        userAccount.PasswordHash = _passwordService.HashPassword(newPassword, out var salt);
        userAccount.PasswordSalt = salt;

        using var dbContext = _dbContextFactory.CreateDbContext();

        dbContext.UserAccounts.Update(userAccount);
        await dbContext.SaveChangesAsync();
    }

    private async Task InvalidateRefreshToken(UserAccount userAccount)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var refreshTokens = await dbContext.GetRefreshTokensByUserIdAsync(userAccount.UserAccountId);
        refreshTokens.ForEach(x => x.IsInvalidated = true);
        dbContext.RefreshTokens.UpdateRange(refreshTokens);
        await dbContext.SaveChangesAsync();
    }
}
