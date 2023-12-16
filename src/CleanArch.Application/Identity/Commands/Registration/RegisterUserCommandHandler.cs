namespace CleanArch.Application.Identity.Commands.Registration;

public class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, Result<RegisterUserCommandResponse>>
{
    private readonly IUserRegistrationService _userRegistrationService;

    public RegisterUserCommandHandler(IUserRegistrationService userRegistrationService)
    {
        _userRegistrationService = userRegistrationService;
    }

    public async ValueTask<Result<RegisterUserCommandResponse>> Handle(RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var isValid = request.TryValidate(out var errors);
        if(!isValid)
        {
            var invalid = Result<RegisterUserCommandResponse>.Invalid(errors);
            return await ValueTask.FromResult(invalid);
        }

        var registerUserResult = await _userRegistrationService.RegisterUserAsync(request.Username,
            request.Email,
            request.Password);

        if(!registerUserResult.IsSuccess)
        {
            var failure = Result<RegisterUserCommandResponse>.Inherit(result: registerUserResult);
            return await ValueTask.FromResult(failure);
        }

        var userAccount = registerUserResult.Value;
        var response = new RegisterUserCommandResponse(userAccount.UserAccountId,
            userAccount.User.Username,
            userAccount.User.Email,
            userAccount.UserRole.ToString());

        var ok = Result<RegisterUserCommandResponse>.Ok(response);
        return await ValueTask.FromResult(ok);
    }
}
