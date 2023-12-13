using Microsoft.EntityFrameworkCore;
using CleanArch.Domain.Entities.Identity;

namespace CleanArch.Application.Common.Extensions.Repository;

public static class UserExtensions
{
    public static async Task<User?> GetUserByUsernameAsync(this IAppDbContext context, string username)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Username == username);
    }

    public static async Task<User?> GetUserByEmailAsync(this IAppDbContext context, string email)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }
}
