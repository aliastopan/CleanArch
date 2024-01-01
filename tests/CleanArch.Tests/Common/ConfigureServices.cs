namespace CleanArch.Tests.Common;

public static class ConfigureService
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<IPasswordService, PasswordService>();

        services.AddDbContext<IAppDbContext, AppDbContext>(options =>
        {
            options.UseInMemoryDatabase($"Database-CleanArch");
        });
        services.AddScoped<IAppDbContextFactory<IAppDbContext>, AppDbContextFactory>();
        services.AddScoped<IAppDbContextSeeder, AppDbContextSeeder>();

        return services;
    }
}
