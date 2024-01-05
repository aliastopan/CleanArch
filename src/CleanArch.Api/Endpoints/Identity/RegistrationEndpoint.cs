using CleanArch.Application.Identity.Commands.Registration;
using CleanArch.Shared.Contracts.Identity;

namespace CleanArch.Api.Endpoints.Identity;

public class RegistrationEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost(ApiEndpoint.Identity.SignUp, SignUp).AllowAnonymous();
    }

    internal async Task<IResult> SignUp([FromServices] ISender sender,
        SignUpRequest request, HttpContext httpContext)
    {
        var result = await sender.Send(new SignUpCommand(request.Username,
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.EmailAddress,
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
