using CleanArch.Application.Common.Interfaces.Persistence;

namespace CleanArch.Api.Extensions;

public static class WebApplicationExtensions
{
    public static void InitializeDbContext(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var task = scope.ServiceProvider.GetRequiredService<IAppDbContextSeeder>().GenerateUsersAsync();
        task.GetAwaiter().GetResult();
    }
}
