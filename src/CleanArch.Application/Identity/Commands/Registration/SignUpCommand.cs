using System.ComponentModel.DataAnnotations;

namespace CleanArch.Application.Identity.Commands.Registration;

public record SignUpCommand : IRequest<Result<SignUpCommandResponse>>
{
    public SignUpCommand(string username,
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        string email,
        string password)
    {
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Email = email;
        Password = password;
    }

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
    public string Email { get; init; }

    [Required]
    [RegularExpression(RegexPattern.StrongPassword)]
    public string Password { get; init; }
}
