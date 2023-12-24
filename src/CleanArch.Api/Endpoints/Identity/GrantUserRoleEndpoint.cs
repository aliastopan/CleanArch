using CleanArch.Application.Identity.Commands.UserRole.Grant;
using CleanArch.Contracts.Identity;

namespace CleanArch.Api.Endpoints.Identity;

public class GrantUserRoleEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost(ApiEndpoint.Identity.GrantRole, GrantUserRole)
            .RequireAuthorization(Policies.AdministratorPrivilege);
    }

    internal async Task<IResult> GrantUserRole([FromServices] ISender sender,
        GrantUserRoleRequest request, HttpContext httpContext)
    {
        var result = await sender.Send(new GrantUserRoleCommand(request.AuthorityAccountId,
            request.AccessPassword,
            request.SubjectAccountId,
            request.Role));

        return result.Match(() => Results.Ok(),
            error => error.AsProblem(new ProblemDetails
            {
                Title = "Failed to Grant User Role"
            },
            context: httpContext));
    }
}
