using CleanArch.Infrastructure.Security;

namespace CleanArch.Tests.Common;

public static class ConfigureService
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConfiguration>(configuration);

        services.Configure<InfrastructureSecretSettings>(configuration.GetSection(InfrastructureSecretSettings.SectionName));
        services.Configure<SecurityTokenSettings>(configuration.GetSection(SecurityTokenSettings.SectionName));

        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ISecurityTokenService, SecurityTokenService>();
        services.AddScoped<ISecurityTokenValidatorService, SecurityTokenValidatorService>();

        services.AddDbContext<IAppDbContext, AppDbContext>(options =>
        {
            options.UseInMemoryDatabase($"Database-CleanArch");
        });
        services.AddScoped<IAppDbContextFactory<IAppDbContext>, AppDbContextFactory>();
        services.AddScoped<IAppDbContextSeeder, AppDbContextSeeder>();

        return services;
    }
}
