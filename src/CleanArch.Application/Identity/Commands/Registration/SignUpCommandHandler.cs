using CleanArch.Application.Common.Interfaces.Managers;

namespace CleanArch.Application.Identity.Commands.Registration;

public class SignUpCommandHandler : IRequestHandler<SignUpCommand, Result<SignUpCommandResponse>>
{
    private readonly IIdentityManager _identityManager;

    public SignUpCommandHandler(IIdentityManager identityManager)
    {
        _identityManager = identityManager;
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
        var trySignUp = await _identityManager.TrySignUpAsync(request.Username,
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.EmailAddress,
            request.Password);

        if(!trySignUp.IsSuccess)
        {
            var failure = Result<SignUpCommandResponse>.Inherit(result: trySignUp);
            return await ValueTask.FromResult(failure);
        }

        var userAccount = trySignUp.Value;
        var response = new SignUpCommandResponse(userAccount.UserAccountId,
            userAccount.User.Username,
            userAccount.UserProfile.FullName,
            userAccount.UserProfile.DateOfBirth,
            userAccount.User.EmailAddress,
            userAccount.UserRoles.Select(role => role.ToString()).ToList());

        var ok = Result<SignUpCommandResponse>.Ok(response);
        return await ValueTask.FromResult(ok);
    }
}
