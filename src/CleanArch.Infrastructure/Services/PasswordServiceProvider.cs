using System.Security.Cryptography;
using System.Text;
using SecretSauce = CCred.Sauce;

namespace CleanArch.Infrastructure.Services;

internal sealed class PasswordServiceProvider : IPasswordService
{
    private readonly Encoding _encoding = Encoding.UTF8;

    public string HashPassword(string rawInput, out string salt)
    {
        salt = SecretSauce.GenerateSalt(length: 8);
        return SecretSauce.GetHash<SHA384>(rawInput, salt, _encoding);
    }

    public bool VerifyPassword(string rawInput, string salt, string hashedInput)
    {
        return SecretSauce.Verify<SHA384>(rawInput, salt, hashedInput, _encoding);
    }
}
