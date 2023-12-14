#nullable disable
using CleanArch.Domain.Enums;

namespace CleanArch.Domain.Entities.Identity;

public class User
{
    public User()
    {
        Role = UserRole.Standard;
        CreationDate = DateTimeOffset.Now;
        LastLoggedIn = DateTimeOffset.Now;
    }

    public User(string username, string email, string passwordHash, string passwordSalt)
    {
        UserId = Guid.NewGuid();
        Username = username;
        Email = email;
        Role = UserRole.Standard;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        CreationDate = DateTimeOffset.Now;
        LastLoggedIn = DateTimeOffset.Now;
    }

    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }

    public UserRole Role { get; set; }
    public bool IsVerified { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset LastLoggedIn { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
}
