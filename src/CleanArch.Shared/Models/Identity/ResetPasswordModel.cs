#nullable disable
using System.ComponentModel.DataAnnotations;
using CleanArch.Shared.Validations;

namespace CleanArch.Shared.Models.Identity;

public class ResetPasswordModel
{
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
