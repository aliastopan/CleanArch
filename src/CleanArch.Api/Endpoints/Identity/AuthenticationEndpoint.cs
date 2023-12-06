using CleanArch.Application.Identity.Commands.Authentication;
using CleanArch.Contracts.Identity;

namespace CleanArch.Api.Endpoints.Identity;

public class AuthenticationEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost("/api/login", Login);
    }

    internal async Task<IResult> Login([FromServices] ISender sender,
        LoginRequest request, HttpContext httpContext)
    {
        var result = await sender.Send(new LoginCommand(request.Username, request.Password));

        return result.Match(
            value =>
            {
                var cookieOption = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddMonths(1)
                };
                httpContext.Response.Cookies.Append("access-token", value.AccessToken, cookieOption);
                return Results.Ok(value);
            },
            fault => fault.AsProblem(new ProblemDetails
            {
                Title = "Authentication Failed"
            },
            context: httpContext));
    }
}
