using System.ComponentModel.DataAnnotations;
using CleanArch.Application.Common.Validations;

namespace CleanArch.Application.Identity.Commands.Registration;

public record RegisterUserCommand : IRequest<Result<RegisterUserCommandResponse>>
{
    public RegisterUserCommand(string username, string email, string password)
    {
        Username = username;
        Email = email;
        Password = password;
    }

    [Required]
    [RegularExpression(RegexPattern.Username)]
    public string Username { get; init; }

    [Required]
    [EmailAddress]
    public string Email { get; init; }

    [Required]
    [RegularExpression(RegexPattern.StrongPassword)]
    public string Password { get; init; }
}
