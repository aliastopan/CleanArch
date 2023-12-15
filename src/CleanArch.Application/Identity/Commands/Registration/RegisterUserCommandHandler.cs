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
        Result<RegisterUserCommandResponse> result;

        var isValid = request.TryValidate(out var errors);
        if(!isValid)
        {
            result = Result<RegisterUserCommandResponse>.Invalid(errors);
            return await ValueTask.FromResult(result);
        }

        var registerUserResult = await _userRegistrationService.RegisterUserAsync(request.Username,
            request.Email,
            request.Password);

        if(!registerUserResult.IsSuccess)
        {
            result = Result<RegisterUserCommandResponse>.Inherit(result: registerUserResult);
            return await ValueTask.FromResult(result);
        }

        var userAccount = registerUserResult.Value;
        var response = new RegisterUserCommandResponse(userAccount.UserAccountId,
            userAccount.User.Username,
            userAccount.User.Email,
            userAccount.UserRole.ToString());

        result = Result<RegisterUserCommandResponse>.Ok(response);
        return await ValueTask.FromResult(result);
    }
}
