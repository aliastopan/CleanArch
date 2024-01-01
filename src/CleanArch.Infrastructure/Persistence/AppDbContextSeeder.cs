using CleanArch.Domain.Aggregates.Identity;
using CleanArch.Domain.Enums;

[assembly: InternalsVisibleTo("CleanArch.Tests")]
namespace CleanArch.Infrastructure.Persistence;

internal sealed class AppDbContextSeeder : IAppDbContextSeeder
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;
    private readonly IPasswordService _passwordService;
    private readonly IDateTimeService _dateTimeService;

    public AppDbContextSeeder(IAppDbContextFactory<IAppDbContext> dbContextFactory,
        IPasswordService passwordService,
        IDateTimeService dateTimeService)
    {
        _dbContextFactory = dbContextFactory;
        _passwordService = passwordService;
        _dateTimeService = dateTimeService;
    }

    public async Task<int> GenerateUsersAsync()
    {
        var userAccount01 = new UserAccount
        {
            UserAccountId = Guid.Parse("9dd0aa01-3a6e-4159-8c7b-8ee4caa1d4ea"),
            User = new User
            {
                Username = "aliastopan",
                EmailAddress = "alias.topan@proton.me"
            },
            UserProfile = new UserProfile
            {
                FirstName = "Taufan",
                LastName = "Augusta",
                DateOfBirth = new DateOnly(year: 1996, month: 8, day: 19)
            },
            PasswordHash = _passwordService.HashPassword("LongPassword012", out var salt),
            PasswordSalt = salt,
            IsVerified = true,
            UserRoles = new List<UserRole>()
            {
                UserRole.Viewer,
                UserRole.Editor,
                UserRole.Manager,
                UserRole.Administrator
            },
            CreationDate = _dateTimeService.DateTimeOffsetNow,
            LastSignedIn = _dateTimeService.DateTimeOffsetNow
        };

        var userAccount02 = new UserAccount
        {
            UserAccountId = Guid.Parse("e55204de-4de4-4101-91b7-672d3b9e5de2"),
            User = new User
            {
                Username = "vincent",
                EmailAddress = "vincent.arkel@email"
            },
            UserProfile = new UserProfile
            {
                FirstName = "Vincent",
                LastName = "Arkel",
                DateOfBirth = new DateOnly(year: 1995, month: 8, day: 19)
            },
            PasswordHash = _passwordService.HashPassword("LongPassword012", out salt),
            PasswordSalt = salt,
            IsVerified = true,
            UserRoles = new List<UserRole>()
            {
                UserRole.Viewer
            },
            CreationDate = _dateTimeService.DateTimeOffsetNow,
            LastSignedIn = _dateTimeService.DateTimeOffsetNow
        };

        var userAccount03 = new UserAccount
        {
            UserAccountId = Guid.Parse("a008959f-b4ef-4284-8ea4-fc88802e3b37"),
            User = new User
            {
                Username = "aram",
                EmailAddress = "aram@email"
            },
            UserProfile = new UserProfile
            {
                FirstName = "Aram",
                LastName = "",
                DateOfBirth = new DateOnly(year: 1995, month: 8, day: 19)
            },
            PasswordHash = _passwordService.HashPassword("LongPassword012", out salt),
            PasswordSalt = salt,
            IsVerified = true,
            UserRoles = new List<UserRole>()
            {
                UserRole.Viewer
            },
            CreationDate = _dateTimeService.DateTimeOffsetNow,
            LastSignedIn = _dateTimeService.DateTimeOffsetNow
        };

        using var dbContext = _dbContextFactory.CreateDbContext();

        dbContext.UserAccounts.Add(userAccount01);
        dbContext.UserAccounts.Add(userAccount02);
        dbContext.UserAccounts.Add(userAccount03);
        return await dbContext.SaveChangesAsync();
    }
}
