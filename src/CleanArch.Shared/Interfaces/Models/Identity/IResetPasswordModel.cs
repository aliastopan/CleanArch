namespace CleanArch.Shared.Interfaces.Models.Identity;

public interface IResetPasswordModel
{
    Guid UserAccountId { get; }
    string OldPassword { get; }
    string NewPassword { get; }
    string ConfirmPassword { get; }
}
