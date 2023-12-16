using System.ComponentModel.DataAnnotations;

namespace CleanArch.Application.Identity.Commands.Authentication;

public record AuthenticateUserCommand : IRequest<Result<AuthenticateUserCommandResponse>>
{
    public AuthenticateUserCommand(string username, string password)
    {
        Username = username;
        Password = password;
    }

    [Required] public string Username { get; init; }
    [Required] public string Password { get; init; }
}
