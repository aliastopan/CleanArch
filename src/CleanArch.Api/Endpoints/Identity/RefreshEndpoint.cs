using System.Net;
using CleanArch.Application.Identity.Commands.Authentication.Refresh;

namespace CleanArch.Api.Endpoints.Identity;

public class RefreshEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost("/api/auth/refresh", Refresh).AllowAnonymous();
    }

    internal async Task<IResult> Refresh([FromServices] ISender sender,
        HttpContext httpContext)
    {
        var accessToken = httpContext.Request.Cookies["access-token"];
        var refreshToken = httpContext.Request.Cookies["refresh-token"];
        if(accessToken is null || refreshToken is null)
        {
            return Results.Problem(new ProblemDetails
            {
                Status = (int)HttpStatusCode.NotFound
            });
        }

        var result = await sender.Send(new RefreshCommand(accessToken, refreshToken));

        return result.Match(
            value =>
            {
                var cookieOption = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddMonths(1)
                };
                httpContext.Response.Cookies.Append("access-token", value.AccessToken, cookieOption);
                httpContext.Response.Cookies.Append("refresh-token", value.RefreshToken, cookieOption);

                return Results.Ok();
            },
            error => error.AsProblem(new ProblemDetails
            {
                Title = "Failed to Refresh Authentication",
            },
            httpContext));
    }
}
