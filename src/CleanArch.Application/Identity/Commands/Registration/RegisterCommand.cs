using System.ComponentModel.DataAnnotations;
using CleanArch.Application.Common.Validations;

namespace CleanArch.Application.Identity.Commands.Registration;

public record RegisterCommand : IRequest<Result<RegisterCommandResponse>>
{
    public RegisterCommand(string username, string email, string password)
    {
        Username = username;
        Email = email;
        Password = password;
    }

    [Required]
    [RegularExpression(RegexPattern.Username)]
    public string Username { get; init; }

    [Required]
    public string Email { get; init; }

    [Required]
    [RegularExpression(RegexPattern.StrongPassword)]
    public string Password { get; init; }
}
