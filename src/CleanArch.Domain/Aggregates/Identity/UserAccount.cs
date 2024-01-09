#nullable disable

namespace CleanArch.Domain.Aggregates.Identity;

public class UserAccount : IAggregateRoot
{
    public UserAccount()
    {

    }

    public UserAccount(string username, string firstName, string lastName,
        DateOnly dateOfBirth, string emailAddress, string passwordHash, string passwordSalt,
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
        UserPrivileges = new List<UserPrivilege>()
        {
            UserPrivilege.Viewer
        };

        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        CreationDate = creationDate;
        LastSignedIn = creationDate;
    }

    public Guid UserAccountId { get; init; }
    public ICollection<UserPrivilege> UserPrivileges { get; set; }
    public bool IsVerified { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public DateTimeOffset CreationDate { get; init; }
    public DateTimeOffset LastSignedIn { get; set; }

    public Guid FkUserId { get; init; }
    public Guid FkUserProfileId { get; init; }

    public virtual User User { get; init; }
    public virtual UserProfile UserProfile { get; init; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; init; }
}
