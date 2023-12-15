#nullable disable
using CleanArch.Domain.Entities.Identity;
using CleanArch.Domain.Enums;

namespace CleanArch.Domain.Aggregates;

public class UserAccount : IAggregateRoot
{
    public UserAccount()
    {

    }

    public UserAccount(string username,
        string email,
        string passwordHash,
        string passwordSalt,
        DateTimeOffset creationDate)
    {
        User = new User
        {
            UserId = Guid.NewGuid(),
            Username = username,
            Email = email
        };

        UserAccountId = Guid.NewGuid();
        UserRole = UserRole.Standard;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        CreationDate = creationDate;
        LastLoggedIn = creationDate;
    }

    public Guid UserAccountId { get; init; }
    public UserRole UserRole { get; set; }
    public bool IsVerified { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public DateTimeOffset CreationDate { get; init; }
    public DateTimeOffset LastLoggedIn { get; set; }

    public virtual User User { get; init; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
}
