using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Infrastructure.Services;

internal sealed class UserRegistrationService : IUserRegistrationService
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;
    private readonly IPasswordService _passwordService;
    private readonly IDateTimeService _dateTimeService;

    public UserRegistrationService(IAppDbContextFactory<IAppDbContext> dbContextFactory,
        IPasswordService passwordService,
        IDateTimeService dateTimeService)
    {
        _dbContextFactory = dbContextFactory;
        _passwordService = passwordService;
        _dateTimeService = dateTimeService;
    }

    public async Task<Result<UserAccount>> TryRegisterUserAsync(string username, string email, string password)
    {
        var validateAvailability = await TryValidateAvailabilityAsync(username, email);
        if(!validateAvailability.IsSuccess)
        {
            return Result<UserAccount>.Inherit(result: validateAvailability);
        }

        var userAccount = await CreateUserAccountAsync(username, email, password);
        return Result<UserAccount>.Ok(userAccount);
    }

    private async Task<UserAccount> CreateUserAccountAsync(string username, string email, string password)
    {
        var hash = _passwordService.HashPassword(password, out string salt);
        var userAccount = new UserAccount(username, email, hash, salt, _dateTimeService.DateTimeOffsetNow);

        using var dbContext = _dbContextFactory.CreateDbContext();

        dbContext.UserAccounts.Add(userAccount);
        await dbContext.SaveChangesAsync();

        return userAccount;
    }

    private async Task<Result> TryValidateAvailabilityAsync(string username, string email)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var userAccount = await dbContext.GetUserAccountByUsernameAsync(username);
        if(userAccount is not null)
        {
            var error = new Error("Username is already taken.", ErrorSeverity.Warning);
            return Result.Conflict(error);
        }

        userAccount = await dbContext.GetUserAccountByEmailAsync(email);
        if(userAccount is not null)
        {
            var error = new Error("Email is already in use.", ErrorSeverity.Warning);
            return Result.Conflict(error);
        }

        return Result.Ok();
    }
}
