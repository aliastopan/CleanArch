using System.ComponentModel.DataAnnotations;
using CleanArch.Application.Common.Validations;

namespace CleanArch.Application.Identity.Commands.ResetPassword;

public record ResetPasswordCommand : IRequest<Result>
{
    public ResetPasswordCommand(Guid userAccountId, string oldPassword, string newPassword, string confirmPassword)
    {
        UserAccountId = userAccountId;
        OldPassword = oldPassword;
        NewPassword = newPassword;
        ConfirmPassword = confirmPassword;
    }

    [Required]
    public Guid UserAccountId { get; init; }

    [Required]
    public string OldPassword { get; init; }

    [Required]
    [RegularExpression(RegexPattern.StrongPassword)]
    public string NewPassword { get; init; }

    [Required]
    [Compare(nameof(NewPassword), ErrorMessage = "Confirm password does not match.")]
    public string ConfirmPassword { get; init; }
}
