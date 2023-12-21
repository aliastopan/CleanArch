using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Application.Identity.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IIdentityService _identityService;

    public ResetPasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async ValueTask<Result> Handle(ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // data annotation validations
        var isValid = request.TryValidate(out var errors);
        if(!isValid)
        {
            var invalid = Result.Invalid(errors);
            return await ValueTask.FromResult(invalid);
        }

        // reset password
        var tryResetPassword = await _identityService.TryResetPasswordAsync(request.UserAccountId, request.OldPassword, request.NewPassword);
        if(!tryResetPassword.IsSuccess)
        {
            var failure = Result.Inherit(result: tryResetPassword);
            return await ValueTask.FromResult(failure);
        }

        var ok = Result.Ok();
        return await ValueTask.FromResult(ok);
    }
}
