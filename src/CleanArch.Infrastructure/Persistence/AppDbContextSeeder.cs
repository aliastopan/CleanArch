using CleanArch.Domain.Entities.Identity;
using CleanArch.Domain.Enums;

namespace CleanArch.Infrastructure.Persistence;

internal sealed class AppDbContextSeeder : IAppDbContextSeeder
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;
    private readonly IPasswordService _passwordService;

    public AppDbContextSeeder(IAppDbContextFactory<IAppDbContext> dbContextFactory,
        IPasswordService passwordService)
    {
        _dbContextFactory = dbContextFactory;
        _passwordService = passwordService;
    }

    public async Task<int> GenerateUsersAsync()
    {
        var user01 = new User
        {
            UserId = Guid.Parse("9dd0aa01-3a6e-4159-8c7b-8ee4caa1d4ea"),
            Username = "aliastopan",
            Email = "aliastopan@proton.me",
            PasswordHash = _passwordService.HashPassword("LongPassword012", out var salt),
            PasswordSalt = salt,
            IsVerified = true,
            Role = UserRole.Developer,
        };

        var user02 = new User
        {
            UserId = Guid.Parse("e55204de-4de4-4101-91b7-672d3b9e5de2"),
            Username = "libromancer",
            Email = "libromancer@email",
            PasswordHash = _passwordService.HashPassword("LongPassword012", out salt),
            PasswordSalt = salt,
            IsVerified = false,
            Role = UserRole.Standard,
        };

        var user03 = new User
        {
            UserId = Guid.Parse("a008959f-b4ef-4284-8ea4-fc88802e3b37"),
            Username = "vanquishsoul",
            Email = "vanquish.soul@email",
            PasswordHash = _passwordService.HashPassword("LongPassword012", out salt),
            PasswordSalt = salt,
            IsVerified = false,
            Role = UserRole.Standard,
        };

        using var dbContext = _dbContextFactory.CreateDbContext();

        dbContext.Users.Add(user01);
        dbContext.Users.Add(user02);
        dbContext.Users.Add(user03);
        return await dbContext.SaveChangesAsync();
    }
}
