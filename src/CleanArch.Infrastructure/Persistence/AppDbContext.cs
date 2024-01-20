using System.Reflection;
using Microsoft.EntityFrameworkCore;
using CleanArch.Domain.Aggregates.Identity;

[assembly: InternalsVisibleTo("CleanArch.Tests")]
namespace CleanArch.Infrastructure.Persistence;

internal sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }

    // aggregates
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();

    // entities
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


    public override int SaveChanges()
    {
        return base.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
