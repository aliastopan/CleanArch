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

        services.AddSingleton<ISecurityTokenValidatorService, SecurityTokenValidatorService>();
        services.AddSingleton<IPasswordService, PasswordService>();
        // services.AddSingleton<IPasswordService, BcryptPasswordService>();
        services.AddScoped<ISecurityTokenService, SecurityTokenService>();


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
