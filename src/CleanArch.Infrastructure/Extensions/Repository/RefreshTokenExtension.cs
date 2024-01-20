using CleanArch.Domain.Aggregates.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Extensions.Repository;

internal static class RefreshTokenExtensions
{
    public static async Task<List<RefreshToken>> GetRefreshTokensByUserAccountIdAsync(this AppDbContext context, Guid userAccountId)
    {
        return await context.RefreshTokens.Where(x => x.FkUserAccountId == userAccountId).ToListAsync();
    }

    public static RefreshToken? GetRefreshToken(this AppDbContext context, string token)
    {
        return context.RefreshTokens
            .Include(x => x.UserAccount)
                .ThenInclude(x => x.User)
            .Include(x => x.UserAccount)
                .ThenInclude(x => x.UserProfile)
            .SingleOrDefault(x => x.Token == token);
    }
}
