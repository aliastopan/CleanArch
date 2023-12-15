namespace CleanArch.Application.Identity.Commands.Registration;

public class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result<RegisterCommandResponse>>
{
    private readonly IUserRegistrationService _userRegistrationService;

    public RegisterCommandHandler(IUserRegistrationService userRegistrationService)
    {
        _userRegistrationService = userRegistrationService;
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

        var registerUserResult = await _userRegistrationService.RegisterUserAsync(request.Username,
            request.Email,
            request.Password);

        if(!registerUserResult.IsSuccess)
        {
            result = Result<RegisterCommandResponse>.Inherit(result: registerUserResult);
            return await ValueTask.FromResult(result);
        }

        var userAccount = registerUserResult.Value;
        var response = new RegisterCommandResponse(userAccount.UserAccountId,
            userAccount.User.Username,
            userAccount.User.Email,
            userAccount.UserRole.ToString());

        result = Result<RegisterCommandResponse>.Ok(response);
        return await ValueTask.FromResult(result);
    }
}
