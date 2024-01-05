using CleanArch.Application.Identity.Commands.Authentication;
using CleanArch.Shared.Contracts.Identity;

namespace CleanArch.Api.Endpoints.Identity;

public class AuthenticationEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost(ApiEndpoint.Identity.SignIn, SignIn).AllowAnonymous();
        app.MapPost(ApiEndpoint.Identity.SignOut, SignOut).AllowAnonymous();
    }

    internal async Task<IResult> SignIn([FromServices] ISender sender,
        SignInRequest request, HttpContext httpContext)
    {
        var result = await sender.Send(new SignInCommand(request.Username, request.Password));

        return result.Match(
            value =>
            {
                var cookieOption = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddMonths(1)
                };
                httpContext.Response.Cookies.Append("access-token", value.AccessToken, cookieOption);
                httpContext.Response.Cookies.Append("refresh-token", value.RefreshTokenStr, cookieOption);
                return Results.Ok(value);
            },
            fault => fault.AsProblem(new ProblemDetails
            {
                Title = "Authentication Failed"
            },
            context: httpContext));
    }

    internal IResult SignOut(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete("access-token");
        httpContext.Response.Cookies.Delete("refresh-token");

        return Results.Ok();
    }
}
