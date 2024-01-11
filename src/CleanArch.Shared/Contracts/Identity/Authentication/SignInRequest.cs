using CleanArch.Shared.Models.Identity;

namespace CleanArch.Shared.Contracts.Identity.Authentication;

public class SignInRequest : AuthenticationModel
{
    public SignInRequest(string username, string password)
    {
        base.Username = username;
        base.Password = password;
    }
}
