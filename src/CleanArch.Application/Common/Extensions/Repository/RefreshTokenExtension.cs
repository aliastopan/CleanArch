using CleanArch.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Application.Common.Extensions.Repository;

public static class RefreshTokenExtensions
{
    public static async Task<List<RefreshToken>> GetRefreshTokensByUserIdAsync(this IAppDbContext context, Guid userAccountId)
    {
        return await context.RefreshTokens.Where(x => x.UserAccountId == userAccountId).ToListAsync();
    }
}
