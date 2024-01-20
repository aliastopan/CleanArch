using CleanArch.Application.Common.Interfaces.Services;

namespace CleanArch.Api.Extensions;

public static class WebApplicationExtensions
{
    public static void InitializeDbContext(this WebApplication app)
    {
        var configuration = app.Services.GetRequiredService<IConfiguration>();
        if (!configuration.UseInMemoryDatabase())
        {
            return;
        }

        using var scope = app.Services.CreateScope();
        var task = scope.ServiceProvider.GetRequiredService<IDataSeedingService>().GenerateUsersAsync();
        task.GetAwaiter().GetResult();
    }
}
