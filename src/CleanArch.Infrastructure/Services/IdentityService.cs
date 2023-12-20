using CleanArch.Domain.Aggregates.Identity;
using CleanArch.Domain.Enums;

namespace CleanArch.Infrastructure.Services;

internal sealed class IdentityService : IIdentityService
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;

    public IdentityService(IAppDbContextFactory<IAppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Result> TryGrantRoleAsync(Guid userAccountId, string role)
    {
        var tryGetUserAccount = await TryGetUserAccountAsync(userAccountId);
        if(!tryGetUserAccount.IsSuccess)
        {
            return Result.Inherit(result: tryGetUserAccount);
        }

        var userAccount = tryGetUserAccount.Value;

        var userRole = (UserRole)Enum.Parse(typeof(UserRole), role);
        if(userAccount.UserRoles.Contains(userRole))
        {
            var error = new Error("Cannot have duplicate role.", ErrorSeverity.Warning);
            return Result.Conflict(error);
        }

        using var dbContext = _dbContextFactory.CreateDbContext();

        userAccount.UserRoles.Add(userRole);
        dbContext.UserAccounts.Update(userAccount);
        await dbContext.SaveChangesAsync();

        return Result.Ok();
    }

    private async Task<Result<UserAccount>> TryGetUserAccountAsync(Guid userAccountId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var userAccount = await dbContext.GetUserAccountByIdAsync(userAccountId);
        if(userAccount is null)
        {
            var error = new Error("User not found.", ErrorSeverity.Warning);
            return Result<UserAccount>.NotFound(error);
        }

        return Result<UserAccount>.Ok(userAccount);
    }
}
