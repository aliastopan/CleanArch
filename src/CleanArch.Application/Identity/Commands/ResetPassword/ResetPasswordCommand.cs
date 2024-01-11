using System.ComponentModel.DataAnnotations;
using CleanArch.Shared.Interfaces.Models.Identity;

namespace CleanArch.Application.Identity.Commands.ResetPassword;

public class ResetPasswordCommand(Guid userAccountId, string oldPassword, string newPassword, string confirmPassword)
    : IResetPasswordModel, IRequest<Result>
{
    [Required]
    public Guid UserAccountId { get; init; } = userAccountId;

    [Required]
    public string OldPassword { get; init; } = oldPassword;

    [Required]
    [RegularExpression(RegexPattern.StrongPassword)]
    public string NewPassword { get; init; } = newPassword;

    [Required]
    [Compare(nameof(NewPassword), ErrorMessage = "Confirm password does not match.")]
    public string ConfirmPassword { get; init; } = confirmPassword;
}
