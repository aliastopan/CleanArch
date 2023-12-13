using CleanArch.Domain.Entities.Identity;

namespace CleanArch.Application.Identity.Commands.Registration;

public class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result<RegisterCommandResponse>>
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;
    private readonly IPasswordService _passwordService;

    public RegisterCommandHandler(IAppDbContextFactory<IAppDbContext> dbContextFactory,
        IPasswordService passwordService)
    {
        _dbContextFactory = dbContextFactory;
        _passwordService = passwordService;
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

        var user = await CreateUserAsync(request.Username, request.Email, request.Password);
        var response = new RegisterCommandResponse(user.UserId,
            user.Username,
            user.Email,
            user.Role.ToString());

        result = Result<RegisterCommandResponse>.Ok(response);
        return await ValueTask.FromResult(result);
    }

    private async Task<Result> ValidateAvailabilityAsync(string username, string email)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var user = await dbContext.GetUserByUsernameAsync(username);
        if(user is not null)
        {
            var error = new Error("Username is already taken.", ErrorSeverity.Warning);
            return Result.Conflict(error);
        }

        user = await dbContext.GetUserByEmailAsync(email);
        if(user is not null)
        {
            var error = new Error("Email is already in use.", ErrorSeverity.Warning);
            return Result.Conflict(error);
        }

        return Result.Ok();
    }

    private async Task<User> CreateUserAsync(string username, string email, string password)
    {
        var hash = _passwordService.HashPassword(password, out string salt);
        var user = new User(username, email, hash, salt);

        using var dbContext = _dbContextFactory.CreateDbContext();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return user;
    }
}
