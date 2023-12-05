using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Persistence;

internal sealed class AppDbContextFactory : IAppDbContextFactory<AppDbContext>
{
    private readonly DbContextOptions<AppDbContext> _options;

    public AppDbContextFactory(DbContextOptions<AppDbContext> options)
    {
        _options = options;
    }

    public AppDbContext CreateDbContext()
    {
        return new AppDbContext(_options);
    }
}
