using Microsoft.AspNetCore.Authorization;

namespace CleanArch.Api.Security.Requirements;

public class VerifiedUserRequirementHandler : AuthorizationHandler<VerifiedUserRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        VerifiedUserRequirement requirement)
    {
        if(context.User.HasClaim(c => c.Type == "is_verified" && c.Value == "true"))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
