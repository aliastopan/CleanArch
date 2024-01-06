using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, ServiceScope scope)
    {
        if (scope is ServiceScope.API_ONLY_SERVICE)
        {
            Log.Warning("Application:API-ONLY SERVICE");
            services.AddMediator(options =>
            {
                options.Namespace = "CleanArch.SourceGeneration.Mediator";
                options.ServiceLifetime = ServiceLifetime.Scoped;
            });
        }

        return services;
    }
}
