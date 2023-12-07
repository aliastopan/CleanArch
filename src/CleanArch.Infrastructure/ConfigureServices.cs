using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CleanArch.Infrastructure.Services;

namespace CleanArch.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<UserSecrets>(configuration.GetSection(UserSecrets.SectionName));
        services.Configure<SecurityTokenSettings>(configuration.GetSection(SecurityTokenSettings.SectionName));

        services.AddSingleton<ISecurityTokenValidatorService, SecurityTokenValidatorServiceProvider>();
        services.AddSingleton<IPasswordService, PasswordServiceProvider>();
        services.AddScoped<ISecurityTokenService, SecurityTokenServiceProvider>();

        services.AddDbContext(configuration);

        return services;
    }

    internal static IServiceCollection AddDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        if(configuration.UseInMemoryDatabase())
        {
            services.AddDbContext<IAppDbContext, AppDbContext>(options =>
            {
                options.UseInMemoryDatabase($"Database-CleanArch");
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IAppDbContextFactory<IAppDbContext>, AppDbContextFactory>();
        }
        else
        {
            throw new NotImplementedException();
        }

        return services;
    }
}
