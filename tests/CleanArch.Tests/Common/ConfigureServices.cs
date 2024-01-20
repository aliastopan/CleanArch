using CleanArch.Infrastructure.Security;

namespace CleanArch.Tests.Common;

public static class ConfigureService
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConfiguration>(configuration);

        services.Configure<AppSecretSettings>(configuration.GetSection(AppSecretSettings.SectionName));
        services.Configure<SecurityTokenSettings>(configuration.GetSection(SecurityTokenSettings.SectionName));

        services.AddScoped<IDateTimeService, DateTimeProvider>();
        services.AddScoped<IPasswordService, PasswordProvider>();
        services.AddScoped<IAccessTokenService, AccessTokenProvider>();
        services.AddScoped<IRefreshTokenService, RefreshTokenProvider>();
        services.AddScoped<ISecurityTokenValidatorService, SecurityTokenValidatorProvider>();

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase($"Database-CleanArch");
        });
        services.AddScoped<AppDbContextFactory>();
        services.AddScoped<IAppDbContextSeeder, AppDbContextSeeder>();

        return services;
    }
}
