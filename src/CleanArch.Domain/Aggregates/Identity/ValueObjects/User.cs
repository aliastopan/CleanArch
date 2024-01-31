#nullable disable

namespace CleanArch.Domain.Aggregates.Identity.ValueObjects;

public class User : ValueObject
{
    public string Username { get; init; }
    public string EmailAddress { get; init; }
    public UserRole UserRole { get; init; }
    public IReadOnlyCollection<UserPrivilege> UserPrivileges { get; init; }

    public User ChangeEmailAddress(string emailAddress)
    {
        return new User
        {
            Username = this.Username,
            EmailAddress = emailAddress,
            UserRole = this.UserRole,
            UserPrivileges = this.UserPrivileges
        };
    }

    public User ChangeUserRole(UserRole userRole)
    {
        return new User
        {
            Username = this.Username,
            EmailAddress = this.EmailAddress,
            UserRole = userRole,
            UserPrivileges = this.UserPrivileges
        };
    }

    public User AddPrivilege(UserPrivilege privilege)
    {
        var privileges = new List<UserPrivilege>(this.UserPrivileges);
        privileges.Add(privilege);

        return new User
        {
            Username = this.Username,
            EmailAddress = this.EmailAddress,
            UserRole = this.UserRole,
            UserPrivileges = privileges
        };
    }

    public User RemovePrivilege(UserPrivilege privilege)
    {
        var privileges = new List<UserPrivilege>(this.UserPrivileges);
        privileges.Remove(privilege);

        return new User
        {
            Username = this.Username,
            EmailAddress = this.EmailAddress,
            UserRole = this.UserRole,
            UserPrivileges = privileges
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Username;
        yield return EmailAddress;
        yield return UserRole;
        yield return UserPrivileges;
    }
}
