using System.ComponentModel.DataAnnotations;

namespace CleanArch.Application.Identity.Commands.Authentication;

public record SignInCommand : IRequest<Result<SignInCommandResponse>>
{
    public SignInCommand(string username, string password)
    {
        Username = username;
        Password = password;
    }

    [Required] public string Username { get; init; }
    [Required] public string Password { get; init; }
}
