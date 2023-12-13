using CleanArch.Contracts.Identity;
using CleanArch.Application.Identity.Commands.ResetPassword;

namespace CleanArch.Api.Endpoints.Identity;

public class ResetPasswordEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost("/api/auth/reset-password", ResetPassword);
    }

    internal async Task<IResult> ResetPassword([FromServices] ISender sender,
        ResetPasswordRequest request, HttpContext httpContext)
    {
        var result = await sender.Send(new ResetPasswordCommand(request.UserId,
            request.OldPassword,
            request.NewPassword,
            request.ConfirmPassword));

        return result.Match(() => Results.Ok(),
            error => error.AsProblem(new ProblemDetails
            {
                Title = "Failed to Reset Password"
            },
            context: httpContext));
    }
}
