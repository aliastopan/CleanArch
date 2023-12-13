namespace CleanArch.Api.Endpoints.Common;

public class DebugEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.Map("api/debug", Debug);
    }

    internal void Debug(HttpContext httpContext)
    {
        var user = httpContext.User;
        foreach(var claim in user.Claims)
        {
            Log.Warning("Claim Type: {0}, Claim Value: {1}", claim.Type, claim.Value);
        }
    }
}
