using CleanArch.Application.Identity.Queries.GetUserAccounts;

namespace CleanArch.Api.Endpoints.Identity;

public class GetUsersEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet(ApiEndpoint.Identity.GetUsers, GetUsers).RequireAuthorization(Policies.VerifiedUserPolicy);
    }

    internal async Task<IResult> GetUsers([FromServices] ISender sender,
        HttpContext httpContext)
    {
        var request = new GetUserAccountsQuery();
        var result = await sender.Send(request);

        return result.Match(
            value => Results.Ok(value),
            error => error.AsProblem(new ProblemDetails
            {
                Title = "Not Found"
            },
            context: httpContext));
    }
}

