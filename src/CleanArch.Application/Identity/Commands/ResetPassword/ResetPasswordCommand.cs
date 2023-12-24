using CleanArch.Shared.Models.Identity;

namespace CleanArch.Application.Identity.Commands.ResetPassword;

public class ResetPasswordCommand : ResetPasswordModel, IRequest<Result>
{
    public ResetPasswordCommand(Guid userAccountId, string oldPassword, string newPassword, string confirmPassword)
    {
        base.UserAccountId = userAccountId;
        base.OldPassword = oldPassword;
        base.NewPassword = newPassword;
        base.ConfirmPassword = confirmPassword;
    }
}
