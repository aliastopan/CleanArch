#nullable disable

namespace CleanArch.Domain.Aggregates.Identity;

public class UserAccount : IAggregateRoot
{
    public UserAccount()
    {

    }

    public UserAccount(string username,
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        string emailAddress,
        string passwordHash,
        string passwordSalt,
        DateTimeOffset creationDate)
    {
        User = new User
        {
            UserId = Guid.NewGuid(),
            Username = username,
            EmailAddress = emailAddress
        };

        UserProfile = new UserProfile
        {
            UserProfileId = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth
        };

        UserAccountId = Guid.NewGuid();
        UserRoles = new List<UserRole>()
        {
            UserRole.Viewer
        };
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        CreationDate = creationDate;
        LastSignedIn = creationDate;
    }

    public Guid UserAccountId { get; init; }
    public ICollection<UserRole> UserRoles { get; set; }
    public bool IsVerified { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public DateTimeOffset CreationDate { get; init; }
    public DateTimeOffset LastSignedIn { get; set; }

    public virtual User User { get; init; }
    public virtual UserProfile UserProfile { get; init; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
}
