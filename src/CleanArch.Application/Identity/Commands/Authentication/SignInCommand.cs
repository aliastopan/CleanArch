using CleanArch.Shared.Models.Identity;

namespace CleanArch.Application.Identity.Commands.Authentication;

public class SignInCommand : UserAuthenticationModel, IRequest<Result<SignInCommandResponse>>
{
    public SignInCommand(string username, string password)
    {
        base.Username = username;
        base.Password = password;
    }
}
