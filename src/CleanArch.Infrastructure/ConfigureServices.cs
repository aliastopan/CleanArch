using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CleanArch.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, Scope scope,
        HostBuilderContext context)
    {
        var configuration = context.Configuration;
        var environment = context.HostingEnvironment;

        services.AddScoped<IMailService, MailService>();

        if (scope is Scope.API_ONLY_SERVICE)
        {
            Log.Warning("Infrastructure:API-ONLY SERVICE");
            services.AddDbContext(configuration, environment);

            services.AddScoped<IIdentityAggregateService, IdentityAggregateService>();

            services.AddScoped<IIdentityManager, IdentityManager>();
            services.AddScoped<IAuthenticationManager, AuthenticationManager>();

            services.Configure<UserSecretSettings>(configuration.GetSection(UserSecretSettings.SectionName));
            services.Configure<SecurityTokenSettings>(configuration.GetSection(SecurityTokenSettings.SectionName));

            services.AddSingleton<ISecurityTokenValidatorService, SecurityTokenValidatorService>();
            services.AddSingleton<IDateTimeService, DateTimeService>();
            services.AddSingleton<IPasswordService, PasswordService>();
            // TODO: Replace password service with Bcrypt
            // services.AddSingleton<IPasswordService, BcryptPasswordService>();
            services.AddScoped<ISecurityTokenService, SecurityTokenService>();

        }

        if (scope is Scope.WEBAPP_ONLY_SERVICE)
        {
            Log.Warning("Infrastructure:WEBAPP-ONLY SERVICE");
        }

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
