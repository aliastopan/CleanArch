namespace CleanArch.Contracts.Identity;

public class SignInRequest : AuthenticationModel
{
    public SignInRequest(string username, string password)
    {
        base.Username = username;
        base.Password = password;
    }
}
