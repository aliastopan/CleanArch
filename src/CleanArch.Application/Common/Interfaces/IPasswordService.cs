namespace CleanArch.Application.Common.Interfaces;

public interface IPasswordService
{
    string HashPassword(string rawPassword, out string salt);
    bool VerifyPassword(string rawPassword, string salt, string hashedPassword);
}
