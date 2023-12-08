namespace CleanArch.Infrastructure.Services;

internal sealed class BcryptPasswordService : IPasswordService
{
    public string HashPassword(string password, out string passwordSalt)
    {
        throw new NotImplementedException();
    }

    public bool VerifyPassword(string password, string passwordSalt, string passwordHash)
    {
        throw new NotImplementedException();
    }
}
