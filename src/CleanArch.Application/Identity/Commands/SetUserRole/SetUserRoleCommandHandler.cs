using CleanArch.Domain.Entities.Identity;
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

        var grantor = await dbContext.GetUserByIdAsync(request.GrantorId);
        var grantee = await dbContext.GetUserByIdAsync(request.GranteeId);

        if(grantor is null || grantee is null)
        {
            var error = new Error("User does not exist.", ErrorSeverity.Warning);
            result = Result.NotFound(error);
            return await ValueTask.FromResult(result);
        }

        var validatePermission = ValidatePermission(grantor!, request.PermissionPassword);
        if(!validatePermission.IsSuccess)
        {
            result = Result.Inherit(result: validatePermission);
            return await ValueTask.FromResult(result);
        }

        await SetUserRoleAsync(grantee, (UserRole)request.Role);

        result = Result.Ok();
        return await ValueTask.FromResult(result);
    }

    private Result ValidatePermission(User user, string password)
    {
        var isVerified = _passwordService.VerifyPassword(password, user.PasswordSalt, user.PasswordHash);
        if(!isVerified)
        {
            var error = new Error("Incorrect password.", ErrorSeverity.Warning);
            return Result.Unauthorized(error);
        }

        var hasPermission = user.Role == UserRole.Developer;
        if(!hasPermission)
        {
            var error = new Error("You don't have permission.", ErrorSeverity.Warning);
            return Result.Forbidden(error);
        }

        return Result.Ok();
    }

    private async Task SetUserRoleAsync(User user, UserRole role)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        user.Role = role;
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();
    }
}
