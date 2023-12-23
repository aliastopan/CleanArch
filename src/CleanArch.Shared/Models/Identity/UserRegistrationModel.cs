#nullable disable
using System.ComponentModel.DataAnnotations;
using CleanArch.Shared.Validations;

namespace CleanArch.Shared.Models.Identity;

public class UserRegistrationModel
{
    [Required]
    [RegularExpression(RegexPattern.Username)]
    public string Username { get; init; }

    [Required]
    [RegularExpression(RegexPattern.NameFormat)]
    [MaxLength(64)]
    public string FirstName { get; init; }

    [MaxLength(64)]
    [RegularExpression(RegexPattern.NameFormat)]
    public string LastName { get; init; }

    [Required]
    public DateOnly DateOfBirth { get; init; }

    [Required]
    [EmailAddress]
    public string EmailAddress { get; init; }

    [Required]
    [RegularExpression(RegexPattern.StrongPassword)]
    public string Password { get; init; }
}
