using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql;

namespace CleanArch.Infrastructure.Persistence;

public static class ConfigurePersistence
{
    internal static IServiceCollection ConfigureDbContext(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
    {
        if (environment.IsDevelopment() && configuration.UseInMemoryDatabase())
        {
            services.AddDbContext<IAppDbContext, AppDbContext>(options =>
            {
                options.UseInMemoryDatabase($"Database-CleanArch");
                options.UseLazyLoadingProxies();
                options.EnableSensitiveDataLogging();
            });
            services.AddScoped<IAppDbContextFactory<IAppDbContext>, AppDbContextFactory>();
            services.AddScoped<IAppDbContextSeeder, AppDbContextSeeder>();
        }
        else
        {
            var connectionString = configuration[AppSecretSettings.Section.ConnectionString];
            var serverVersion = new MariaDbServerVersion(new Version(10, 6, 16));

            services.AddDbContext<IAppDbContext, AppDbContext>(options =>
            {
                options.UseMySql(connectionString, serverVersion);
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });
            services.AddScoped<IAppDbContextFactory<IAppDbContext>, AppDbContextFactory>();
        }

        return services;
    }
}
