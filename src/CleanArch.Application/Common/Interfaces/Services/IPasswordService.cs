namespace CleanArch.Application.Common.Interfaces.Services;

public interface IPasswordService
{
    string HashPassword(string password, out string passwordSalt);
    bool VerifyPassword(string password, string passwordSalt, string passwordHash);
}
