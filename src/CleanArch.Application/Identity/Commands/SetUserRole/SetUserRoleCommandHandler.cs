using CleanArch.Domain.Aggregates.Identity;
using CleanArch.Domain.Enums;

namespace CleanArch.Application.Identity.Commands.SetUserRole;

public class SetUserRoleCommandHandler : IRequestHandler<SetUserRoleCommand, Result>
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;
    private readonly IPasswordService _passwordService;

    public SetUserRoleCommandHandler(IAppDbContextFactory<IAppDbContext> dbContextFactory,
        IPasswordService passwordService)
    {
        _dbContextFactory = dbContextFactory;
        _passwordService = passwordService;
    }

    public async ValueTask<Result> Handle(SetUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        Result result;

        var isRoleDefined = Enum.IsDefined(typeof(UserRole), request.Role);
        if(!isRoleDefined)
        {
            var error = new Error("Invalid role request.", ErrorSeverity.Warning);
            result = Result.Invalid(error);
            return await ValueTask.FromResult(result);
        }


        using var dbContext = _dbContextFactory.CreateDbContext();

        var grantorAccount = await dbContext.GetUserAccountByIdAsync(request.GrantorId);
        var granteeAccount = await dbContext.GetUserAccountByIdAsync(request.GranteeId);

        if(grantorAccount is null || granteeAccount is null)
        {
            var error = new Error("User does not exist.", ErrorSeverity.Warning);
            result = Result.NotFound(error);
            return await ValueTask.FromResult(result);
        }

        var validatePermission = ValidatePermission(grantorAccount!, request.PermissionPassword);
        if(!validatePermission.IsSuccess)
        {
            result = Result.Inherit(result: validatePermission);
            return await ValueTask.FromResult(result);
        }

        await SetUserRoleAsync(granteeAccount, (UserRole)request.Role);

        result = Result.Ok();
        return await ValueTask.FromResult(result);
    }

    private Result ValidatePermission(UserAccount userAccount, string password)
    {
        var isVerified = _passwordService.VerifyPassword(password, userAccount.PasswordSalt, userAccount.PasswordHash);
        if(!isVerified)
        {
            var error = new Error("Incorrect password.", ErrorSeverity.Warning);
            return Result.Unauthorized(error);
        }

        var hasPermission = userAccount.UserRole == UserRole.Developer;
        if(!hasPermission)
        {
            var error = new Error("You don't have permission.", ErrorSeverity.Warning);
            return Result.Forbidden(error);
        }

        return Result.Ok();
    }

    private async Task SetUserRoleAsync(UserAccount userAccount, UserRole userRole)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        userAccount.UserRole = userRole;
        dbContext.UserAccounts.Update(userAccount);
        await dbContext.SaveChangesAsync();
    }
}
