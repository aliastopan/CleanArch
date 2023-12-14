using CleanArch.Domain.Entities.Identity;

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

        var user = await dbContext.GetUserByIdAsync(request.UserId);
        if(user is null)
        {
            var error = new Error("User does not exist.", ErrorSeverity.Warning);
            result = Result.NotFound(errors);
            return await ValueTask.FromResult(result);
        }

        var validatePassword = ValidatePassword(request.NewPassword,
            request.OldPassword,
            user.PasswordSalt,
            user.PasswordHash);
        if(!validatePassword.IsSuccess)
        {
            result = Result.Inherit(result: validatePassword);
            return await ValueTask.FromResult(result);
        }

        await ResetPassword(user, request.NewPassword);
        await InvalidateRefreshToken(user);

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

    private async Task ResetPassword(User user, string newPassword)
    {
        user.PasswordHash = _passwordService.HashPassword(newPassword, out var salt);
        user.PasswordSalt = salt;

        using var dbContext = _dbContextFactory.CreateDbContext();

        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();
    }

    private async Task InvalidateRefreshToken(User user)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var refreshTokens = await dbContext.GetRefreshTokensByUserIdAsync(user.UserId);
        refreshTokens.ForEach(x => x.IsInvalidated = true);
        dbContext.RefreshTokens.UpdateRange(refreshTokens);
        await dbContext.SaveChangesAsync();
    }
}
