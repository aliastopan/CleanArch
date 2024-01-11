using CleanArch.Application.Identity.Commands.SetUserRole;
using CleanArch.Shared.Contracts.Identity.SetUserRole;

namespace CleanArch.Api.Endpoints.Identity;

public class SetRoleEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost(ApiEndpoint.Identity.SetRole, SetRole)
            .RequireAuthorization(Policies.AdministratorPrivilege);
    }

    internal async Task<IResult> SetRole([FromServices] ISender sender,
        SetRoleRequest request, HttpContext httpContext)
    {
        var result = await sender.Send(new SetUserRoleCommand(request.AuthorityAccountId,
            request.AccessPassword,
            request.SubjectAccountId,
            request.Role));

        return result.Match(() => Results.Ok(),
            error => error.AsProblem(new ProblemDetails
            {
                Title = "Failed to Update User Role"
            },
            context: httpContext));
    }
}
