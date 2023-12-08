namespace CleanArch.Application.Common.Interfaces;

public interface IPasswordService
{
    string HashPassword(string password, out string passwordSalt);
    bool VerifyPassword(string password, string passwordSalt, string passwordHash);
}
