namespace CleanArch.Shared.Interfaces.Models.Identity;

public interface IAuthenticationModel
{
    string Username { get; }
    string Password { get; }
}
