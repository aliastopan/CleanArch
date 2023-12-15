using CleanArch.Application.Identity.Commands.Registration;
using CleanArch.Contracts.Identity;

namespace CleanArch.Api.Endpoints.Identity;

public class UserRegistrationEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost("/api/register", RegisterUser).AllowAnonymous();
    }

    internal async Task<IResult> RegisterUser([FromServices] ISender sender,
        RegisterRequest request, HttpContext httpContext)
    {
        var result = await sender.Send(new RegisterUserCommand(request.Username,
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
