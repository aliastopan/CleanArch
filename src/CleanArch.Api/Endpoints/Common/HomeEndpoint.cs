namespace CleanArch.Api.Endpoints.Common;

public class HomeEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/", () => "Your princess is in another castle.").AllowAnonymous();
    }
}
