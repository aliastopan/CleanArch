using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Application;

public enum Scope
{
    API_ONLY_SERVICE,
    WEBAPP_ONLY_SERVICE
}

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, Scope scope)
    {
        if (scope is Scope.API_ONLY_SERVICE)
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
