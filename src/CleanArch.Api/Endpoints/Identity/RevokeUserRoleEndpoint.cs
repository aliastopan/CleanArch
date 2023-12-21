using CleanArch.Application.Identity.Commands.UserRole.Revoke;
using CleanArch.Contracts.Identity;

namespace CleanArch.Api.Endpoints.Identity;

public class RevokeUserRoleEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost(ApiEndpoint.Identity.RevokeRole, RevokeUserRole)
            .RequireAuthorization(Policies.AdministratorPrivilege);
    }

    internal async Task<IResult> RevokeUserRole([FromServices] ISender sender,
        GrantUserRoleRequest request, HttpContext httpContext)
    {
        var result = await sender.Send(new RevokeUserRoleCommand(request.SenderAccountId,
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
