using System.Security.Cryptography;
using System.Text;
using SecretSauce = CCred.Sauce;

namespace CleanArch.Infrastructure.Services;

internal sealed class PasswordService : IPasswordService
{
    private readonly Encoding _encoding = Encoding.UTF8;

    public string HashPassword(string password, out string passwordSalt)
    {
        passwordSalt = SecretSauce.GenerateSalt(length: 16);
        return SecretSauce.GetHash<SHA384>(password, passwordSalt, _encoding);
    }

    public bool VerifyPassword(string password, string passwordSalt, string passwordHash)
    {
        return SecretSauce.Verify<SHA384>(password, passwordSalt, passwordHash, _encoding);
    }
}
