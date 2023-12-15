using CleanArch.Domain.Aggregates.Identity;
using CleanArch.Domain.Enums;

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
            User = new User
            {
                UserId = Guid.Parse("9dd0aa01-3a6e-4159-8c7b-8ee4caa1d4ea"),
                Username = "aliastopan",
                Email = "alias.topan@proton.me"
            },
            PasswordHash = _passwordService.HashPassword("LongPassword012", out var salt),
            PasswordSalt = salt,
            IsVerified = true,
            UserRole = UserRole.Developer,
            CreationDate = _dateTimeService.DateTimeOffsetNow,
            LastLoggedIn = _dateTimeService.DateTimeOffsetNow
        };

        var userAccount02 = new UserAccount
        {
            User = new User
            {
                UserId = Guid.Parse("e55204de-4de4-4101-91b7-672d3b9e5de2"),
                Username = "libromancer",
                Email = "libromancer@email"
            },
            PasswordHash = _passwordService.HashPassword("LongPassword012", out salt),
            PasswordSalt = salt,
            IsVerified = true,
            UserRole = UserRole.Standard,
            CreationDate = _dateTimeService.DateTimeOffsetNow,
            LastLoggedIn = _dateTimeService.DateTimeOffsetNow
        };

        var userAccount03 = new UserAccount
        {
            User = new User
            {
                UserId = Guid.Parse("a008959f-b4ef-4284-8ea4-fc88802e3b37"),
                Username = "vanquishsoul",
                Email = "vanquish.soul@email"
            },
            PasswordHash = _passwordService.HashPassword("LongPassword012", out salt),
            PasswordSalt = salt,
            IsVerified = true,
            UserRole = UserRole.Standard,
            CreationDate = _dateTimeService.DateTimeOffsetNow,
            LastLoggedIn = _dateTimeService.DateTimeOffsetNow
        };

        using var dbContext = _dbContextFactory.CreateDbContext();

        dbContext.UserAccounts.Add(userAccount01);
        dbContext.UserAccounts.Add(userAccount02);
        dbContext.UserAccounts.Add(userAccount03);
        return await dbContext.SaveChangesAsync();
    }
}
