using System.ComponentModel.DataAnnotations;
using CleanArch.Shared.Contracts.Identity.Registration;
using CleanArch.Shared.Interfaces.Models.Identity;

namespace CleanArch.Application.Identity.Commands.Registration;

public class SignUpCommand(string username, string firstName, string lastName, DateOnly dateOfBirth, string emailAddress, string password)
    : IRegistrationModel, IRequest<Result<SignUpResponse>>
{
    [Required]
    [RegularExpression(RegexPattern.Username)]
    public string Username { get; } = username;

    [Required]
    [RegularExpression(RegexPattern.NameFormat)]
    [MaxLength(64)]
    public string FirstName { get; } = firstName;

    [MaxLength(64)]
    [RegularExpression(RegexPattern.NameFormat)]
    public string LastName { get; } = lastName;

    [Required]
    public DateOnly DateOfBirth { get; } = dateOfBirth;

    [Required]
    [EmailAddress]
    public string EmailAddress { get; } = emailAddress;

    [Required]
    [RegularExpression(RegexPattern.StrongPassword)]
    public string Password { get; } = password;
}
