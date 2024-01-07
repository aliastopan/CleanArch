using Microsoft.AspNetCore.Authorization;

namespace CleanArch.Api.Middlewares;

public class AnonymousAwarenessMiddleware
{
    private readonly RequestDelegate _next;

    public AnonymousAwarenessMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() is object)
        {
            Log.Warning("Anonymous call of {0}", endpoint);
            await _next(context);

            return;
        }
    }
}
