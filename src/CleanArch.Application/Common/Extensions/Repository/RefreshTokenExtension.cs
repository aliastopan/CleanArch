using CleanArch.Domain.Aggregates.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Application.Common.Extensions.Repository;

public static class RefreshTokenExtensions
{
    public static async Task<List<RefreshToken>> GetRefreshTokensByUserAccountIdAsync(this IAppDbContext context, Guid userAccountId)
    {
        return await context.RefreshTokens.Where(x => x.UserAccountId == userAccountId).ToListAsync();
    }

    public static RefreshToken? GetRefreshToken(this IAppDbContext context, string token)
    {
        return context.RefreshTokens
            .Include(x => x.UserAccount)
                .ThenInclude(x => x.User)
            .Include(x => x.UserAccount)
                .ThenInclude(x => x.UserProfile)
            .SingleOrDefault(x => x.Token == token);
    }
}
