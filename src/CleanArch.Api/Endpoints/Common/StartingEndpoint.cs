namespace CleanArch.Api.Endpoints.Common;

public class StartingEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.Map("api/start", _ => throw new NotImplementedException()).AllowAnonymous();
    }
}
