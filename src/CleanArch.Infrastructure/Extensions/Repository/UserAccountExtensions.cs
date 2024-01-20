using Microsoft.EntityFrameworkCore;
using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Infrastructure.Extensions.Repository;

internal static class UserAccountExtensions
{
    public static async Task<UserAccount?> GetUserAccountByIdAsync(this AppDbContext context, Guid userAccountId)
    {
        return await context.UserAccounts
            .Include(x => x.User)
            .Include(x => x.UserProfile)
            .FirstOrDefaultAsync(x => x.UserAccountId == userAccountId);
    }

    public static async Task<UserAccount?> GetUserAccountByUsernameAsync(this AppDbContext context, string username)
    {
        return await context.UserAccounts
            .Include(x => x.User)
            .Include(x => x.UserProfile)
            .FirstOrDefaultAsync(x => x.User.Username == username);
    }

    public static async Task<UserAccount?> GetUserAccountByEmailAddressAsync(this AppDbContext context, string emailAddress)
    {
        return await context.UserAccounts
            .Include(x => x.User)
            .Include(x => x.UserProfile)
            .FirstOrDefaultAsync(x => x.User.EmailAddress == emailAddress);
    }

    public static async Task<List<UserAccount>> GetUserAccountAsync(this AppDbContext context)
    {
        return await context.UserAccounts
            .Include(x => x.User)
            .Include(x => x.UserProfile)
            .ToListAsync();
    }

    public static async Task<List<UserAccount>> GetRangeUserAccountAsync(this AppDbContext context, int range = 0)
    {
        if (range > 0)
        {
            return await context.UserAccounts
                .Include(x => x.User)
                .Include(x => x.UserProfile)
                .Take(range)
                .ToListAsync();
        }
        else
        {
            return await context.UserAccounts
                .Include(x => x.User)
                .Include(x => x.UserProfile)
                .ToListAsync();
        }
    }
}
