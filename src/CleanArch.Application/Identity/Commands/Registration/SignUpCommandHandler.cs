namespace CleanArch.Application.Identity.Commands.Registration;

public class SignUpCommandHandler
    : IRequestHandler<SignUpCommand, Result<SignUpCommandResponse>>
{
    private readonly IRegistrationService _registrationService;

    public SignUpCommandHandler(IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    public async ValueTask<Result<SignUpCommandResponse>> Handle(SignUpCommand request,
        CancellationToken cancellationToken)
    {
        // data annotation validations
        var isValid = request.TryValidate(out var errors);
        if(!isValid)
        {
            var invalid = Result<SignUpCommandResponse>.Invalid(errors);
            return await ValueTask.FromResult(invalid);
        }

        // registration
        var trySignUp = await _registrationService.TrySignUpAsync(request.Username,
            request.Email,
            request.Password);

        if(!trySignUp.IsSuccess)
        {
            var failure = Result<SignUpCommandResponse>.Inherit(result: trySignUp);
            return await ValueTask.FromResult(failure);
        }

        var userAccount = trySignUp.Value;
        var response = new SignUpCommandResponse(userAccount.UserAccountId,
            userAccount.User.Username,
            userAccount.User.Email,
            userAccount.UserRoles.Select(role => role.ToString()).ToList());

        var ok = Result<SignUpCommandResponse>.Ok(response);
        return await ValueTask.FromResult(ok);
    }
}
