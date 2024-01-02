using CleanArch.Domain.Aggregates.Identity;
using CleanArch.Domain.Enums;

namespace CleanArch.Tests.Common.Factories;

public static class IdentityFactory
{
    public static UserAccount GetTestUserAccount()
    {
        return new UserAccount
        {
            UserAccountId = Guid.Parse("5d771905-c325-4f9a-adb8-954e0ae21860"),
            User = new User
            {
                Username = "tester",
                EmailAddress = "tester@mail.com"
            },
            UserProfile = new UserProfile
            {
                FirstName = "tester",
                LastName = "",
                DateOfBirth = new DateOnly(year: 1999, month: 9, day: 9)
            },
            PasswordHash = "",
            PasswordSalt = "",
            IsVerified = true,
            UserRoles = new List<UserRole>()
            {
                UserRole.Viewer,
                UserRole.Editor,
                UserRole.Manager,
                UserRole.Administrator
            },
            CreationDate = DateTimeOffset.Now,
            LastSignedIn = DateTimeOffset.Now
        };
    }
}
