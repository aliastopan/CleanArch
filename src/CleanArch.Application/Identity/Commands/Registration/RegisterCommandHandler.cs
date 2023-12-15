using CleanArch.Domain.Aggregates;

namespace CleanArch.Application.Identity.Commands.Registration;

public class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result<RegisterCommandResponse>>
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;
    private readonly IPasswordService _passwordService;
    private readonly IDateTimeService _dateTimeService;

    public RegisterCommandHandler(IAppDbContextFactory<IAppDbContext> dbContextFactory,
        IPasswordService passwordService,
        IDateTimeService dateTimeService)
    {
        _dbContextFactory = dbContextFactory;
        _passwordService = passwordService;
        _dateTimeService = dateTimeService;
    }

    public async ValueTask<Result<RegisterCommandResponse>> Handle(RegisterCommand request,
        CancellationToken cancellationToken)
    {
        Result<RegisterCommandResponse> result;

        var isValid = request.TryValidate(out var errors);
        if(!isValid)
        {
            result = Result<RegisterCommandResponse>.Invalid(errors);
            return await ValueTask.FromResult(result);
        }

        var validateAvailability = await ValidateAvailabilityAsync(request.Username, request.Email);
        if(!validateAvailability.IsSuccess)
        {
            result = Result<RegisterCommandResponse>.Inherit(result: validateAvailability);
            return await ValueTask.FromResult(result);
        }

        var userAccount = await CreateUserAccountAsync(request.Username, request.Email, request.Password);
        var response = new RegisterCommandResponse(userAccount.UserAccountId,
            userAccount.User.Username,
            userAccount.User.Email,
            userAccount.UserRole.ToString());

        result = Result<RegisterCommandResponse>.Ok(response);
        return await ValueTask.FromResult(result);
    }

    private async Task<Result> ValidateAvailabilityAsync(string username, string email)
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

    private async Task<UserAccount> CreateUserAccountAsync(string username, string email, string password)
    {
        var hash = _passwordService.HashPassword(password, out string salt);
        var userAccount = new UserAccount(username, email, hash, salt, _dateTimeService.DateTimeOffsetNow);

        using var dbContext = _dbContextFactory.CreateDbContext();

        dbContext.UserAccounts.Add(userAccount);
        await dbContext.SaveChangesAsync();

        return userAccount;
    }
}
