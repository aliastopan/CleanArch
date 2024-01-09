using System.Net;
using CleanArch.Application.Identity.Commands.Authentication.Refresh;
using CleanArch.Shared.Contracts.Identity;

namespace CleanArch.Api.Endpoints.Identity;

public class RefreshAccessEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost(ApiEndpoint.Identity.Refresh, Refresh).AllowAnonymous();
    }

    internal async Task<IResult> Refresh([FromServices] ISender sender,
        RefreshAccessRequest request, HttpContext httpContext)
    {
        var result = await sender.Send(new RefreshAccessCommand(request.AccessToken, request.RefreshTokenStr));

        return result.Match(
            value =>
            {
                return Results.Ok(value);
            },
            error => error.AsProblem(new ProblemDetails
            {
                Title = "Failed to Refresh Authentication",
            },
            httpContext));
    }

    internal async Task<IResult> HttpCookieRefresh([FromServices] ISender sender,
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

        var result = await sender.Send(new RefreshAccessCommand(accessToken, refreshToken));

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
            error => error.AsProblem(new ProblemDetails
            {
                Title = "Failed to Refresh Authentication",
            },
            httpContext));
    }
}
