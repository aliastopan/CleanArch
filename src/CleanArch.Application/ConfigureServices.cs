using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
#if API_ONLY_SERVICE
        services.AddMediator(options =>
        {
            options.Namespace = "CleanArch.SourceGeneration.Mediator";
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });
#endif

        return services;
    }
}
