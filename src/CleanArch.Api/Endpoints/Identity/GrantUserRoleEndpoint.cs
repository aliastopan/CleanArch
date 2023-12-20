using CleanArch.Application.Identity.Commands.UserRole.Grant;
using CleanArch.Contracts.Identity;

namespace CleanArch.Api.Endpoints.Identity;

public class GrantUserRoleEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost("/api/identity/grant-role", GrantUserRole)
            .RequireAuthorization(Policies.AdministratorPrivilege);
    }

    internal async Task<IResult> GrantUserRole([FromServices] ISender sender,
        GrantUserRoleRequest request, HttpContext httpContext)
    {
        var result = await sender.Send(new GrantUserRoleCommand(request.SenderAccountId,
            request.AccessPassword,
            request.RecipientAccountId,
            request.Role));

        return result.Match(() => Results.Ok(),
            error => error.AsProblem(new ProblemDetails
            {
                Title = "Failed to Grant User Role"
            },
            context: httpContext));
    }
}
