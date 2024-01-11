using CleanArch.Shared.Contracts.Identity.Authentication;
using CleanArch.Shared.Models.Identity;

namespace CleanArch.Application.Identity.Commands.Authentication;

public class SignInCommand : AuthenticationModel, IRequest<Result<SignInResponse>>
{
    public SignInCommand(string username, string password)
    {
        base.Username = username;
        base.Password = password;
    }
}
