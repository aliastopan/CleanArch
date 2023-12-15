using Microsoft.EntityFrameworkCore;
using CleanArch.Domain.Aggregates;
using CleanArch.Domain.Entities.Identity;

namespace CleanArch.Application.Common.Interfaces.Persistence;

public interface IAppDbContext : IDisposable
{
    // aggregates
    DbSet<UserAccount> UserAccounts { get; }

    // entities
    DbSet<RefreshToken> RefreshTokens { get; }


    int SaveChanges();
    Task<int> SaveChangesAsync();
}
