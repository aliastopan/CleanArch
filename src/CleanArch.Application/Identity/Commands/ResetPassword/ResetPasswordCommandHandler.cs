using CleanArch.Application.Common.Interfaces.Managers;

namespace CleanArch.Application.Identity.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IIdentityManager _identityManager;

    public ResetPasswordCommandHandler(IIdentityManager identityManager)
    {
        _identityManager = identityManager;
    }

    public async ValueTask<Result> Handle(ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // data annotation validations
        var isInvalid = !request.TryValidate(out var errors);
        if (isInvalid)
        {
            return Result.Invalid(errors);
        }

        // reset password
        var tryResetPassword = await _identityManager.TryResetPasswordAsync(request.UserAccountId,
            request.OldPassword,
            request.NewPassword);

        if (tryResetPassword.IsFailure())
        {
            return Result.Inherit(result: tryResetPassword);
        }

        return Result.Ok();
    }
}
