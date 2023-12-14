using CleanArch.Application.Identity.Commands.Registration;
using CleanArch.Contracts.Identity;

namespace CleanArch.Api.Endpoints.Identity;

public class RegistrationEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost("/api/register", Register).AllowAnonymous();
    }

    internal async Task<IResult> Register([FromServices] ISender sender,
        RegisterRequest request, HttpContext httpContext)
    {
        var result = await sender.Send(new RegisterCommand(request.Username,
            request.Email,
            request.Password)
        );

        return result.Match(
            value => Results.Ok(value),
            error => error.AsProblem(new ProblemDetails
            {
                Title = "Failed to Register User"
            },
            context: httpContext));
    }
}
