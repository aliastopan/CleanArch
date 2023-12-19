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
        await Task.CompletedTask;
        throw new NotImplementedException();
    }

    private Result ValidatePermission(UserAccount userAccount, string password)
    {
        var isVerified = _passwordService.VerifyPassword(password, userAccount.PasswordSalt, userAccount.PasswordHash);
        if(!isVerified)
        {
            var error = new Error("Incorrect password.", ErrorSeverity.Warning);
            return Result.Unauthorized(error);
        }

        var hasPermission = userAccount.UserRoles.Any(role =>
        {
            return (role & UserRole.Administrator) == UserRole.Administrator;
        });
        if(!hasPermission)
        {
            var error = new Error("You don't have permission.", ErrorSeverity.Warning);
            return Result.Forbidden(error);
        }

        return Result.Ok();
    }

    private async Task SetUserRoleAsync(UserAccount userAccount, List<UserRole> userRoles)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        userAccount.UserRoles = userRoles;
        dbContext.UserAccounts.Update(userAccount);
        await dbContext.SaveChangesAsync();
    }
}
