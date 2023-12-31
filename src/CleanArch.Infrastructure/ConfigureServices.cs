using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddScoped<IIdentityAggregateService, IdentityAggregateService>();

        services.AddScoped<IIdentityManager, IdentityManager>();
        services.AddScoped<IAuthenticationManager, AuthenticationManager>();

        services.Configure<UserSecrets>(configuration.GetSection(UserSecrets.SectionName));
        services.Configure<SecurityTokenSettings>(configuration.GetSection(SecurityTokenSettings.SectionName));

        services.AddSingleton<ISecurityTokenValidatorService, SecurityTokenValidatorService>();
        services.AddSingleton<IDateTimeService, DateTimeService>();
        services.AddSingleton<IPasswordService, PasswordService>();
        // TODO: Replace password service with Bcrypt
        // services.AddSingleton<IPasswordService, BcryptPasswordService>();
        services.AddScoped<ISecurityTokenService, SecurityTokenService>();

        services.AddScoped<IMailService, MailService>();

        services.AddDbContext(configuration, environment);

        return services;
    }

    internal static IServiceCollection AddDbContext(this IServiceCollection services,
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
            // TODO: Replace with code for using SQLite
            throw new NotImplementedException();
        }

        return services;
    }
}
