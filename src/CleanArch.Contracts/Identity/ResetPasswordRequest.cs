namespace CleanArch.Contracts.Identity;

public class ResetPasswordRequest : ResetPasswordModel
{
    public ResetPasswordRequest(Guid userAccountId, string oldPassword, string newPassword, string confirmPassword)
    {
        base.UserAccountId = userAccountId;
        base.OldPassword = oldPassword;
        base.NewPassword = newPassword;
        base.ConfirmPassword = confirmPassword;
    }
}
