using Microsoft.EntityFrameworkCore;
using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Application.Common.Extensions.Repository;

public static class UserAccountExtensions
{
    public static async Task<UserAccount?> GetUserAccountByIdAsync(this IAppDbContext context, Guid userAccountId)
    {
        return await context.UserAccounts
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.UserAccountId == userAccountId);
    }

    public static async Task<UserAccount?> GetUserAccountByUsernameAsync(this IAppDbContext context, string username)
    {
        return await context.UserAccounts
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.User.Username == username);
    }

    public static async Task<UserAccount?> GetUserAccountByEmailAsync(this IAppDbContext context, string email)
    {
        return await context.UserAccounts
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.User.Email == email);
    }

    public static async Task<List<UserAccount>> GetUserAccountAsync(this IAppDbContext context)
    {
        return await context.UserAccounts
            .Include(x => x.User)
            .ToListAsync();
    }
}
