using CleanArch.Application.Identity.Commands.SetUserRole;
using CleanArch.Contracts.Identity;

namespace CleanArch.Api.Endpoints.Identity;

public class SetUserRoleEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost("/api/auth/set-role", SetUserRole)
            .RequireAuthorization(Policies.VerifiedUserPolicy)
            .RequireAuthorization(Policies.AdministratorPrivilege);
    }

    internal async Task<IResult> SetUserRole([FromServices] ISender sender,
        SetRoleRequest request, HttpContext httpContext)
    {
        var result = await sender.Send(new SetUserRoleCommand(request.GrantorId,
            request.PermissionPassword,
            request.GranteeId,
            request.Role));

        return result.Match(() => Results.Ok(),
            error => error.AsProblem(new ProblemDetails
            {
                Title = "Failed to Set User Role"
            },
            context: httpContext));
    }
}
