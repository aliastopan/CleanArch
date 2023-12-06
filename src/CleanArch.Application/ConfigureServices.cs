using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediator(options =>
        {
            options.Namespace = "CleanArch.SourceGeneration.Mediator";
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });

        return services;
    }
}
