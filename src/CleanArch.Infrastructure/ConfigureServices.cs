using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CleanArch.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, ServiceScope scope,
        HostBuilderContext context)
    {
        if (scope is ServiceScope.API_ONLY_SERVICE)
        {
            Log.Warning("Infrastructure:API-ONLY SERVICE");
            services.ConfigureApiServices(context.Configuration, context.HostingEnvironment);
        }

        if (scope is ServiceScope.WEBAPP_ONLY_SERVICE)
        {
            Log.Warning("Infrastructure:WEBAPP-ONLY SERVICE");
            services.ConfigureWebAppServices();
        }

        Log.Warning("Infrastructure:COMMON SERVICE");
        services.Configure<InfrastructureSecretSettings>(context.Configuration.GetSection(InfrastructureSecretSettings.SectionName));

        services.AddSingleton<ISecurityTokenValidatorService, SecurityTokenValidatorService>();
        services.AddSingleton<IDateTimeService, DateTimeService>();
        services.AddSingleton<IPasswordService, PasswordService>();
        // TODO: Replace password service with Bcrypt
        // services.AddSingleton<IPasswordService, BcryptPasswordService>();

        services.AddScoped<IAccessTokenService, AccessTokenService>();
        services.AddScoped<IMailService, MailService>();

        return services;
    }

    internal static IServiceCollection ConfigureApiServices(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
    {
        services.ConfigureDataPersistence(configuration, environment);

        services.Configure<SecurityTokenSettings>(configuration.GetSection(SecurityTokenSettings.SectionName));

        services.AddScoped<IIdentityAggregateService, IdentityAggregateService>();
        services.AddScoped<IIdentityManager, IdentityManager>();
        services.AddScoped<IAuthenticationManager, AuthenticationManager>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<ISecurityTokenService, SecurityTokenService>();

        return services;
    }

    internal static IServiceCollection ConfigureWebAppServices(this IServiceCollection services)
    {
        return services;
    }

    internal static IServiceCollection ConfigureDataPersistence(this IServiceCollection services,
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
