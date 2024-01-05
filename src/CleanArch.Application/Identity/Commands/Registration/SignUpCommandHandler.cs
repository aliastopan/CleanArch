using CleanArch.Shared.Contracts.Identity;

namespace CleanArch.Application.Identity.Commands.Registration;

public class SignUpCommandHandler : IRequestHandler<SignUpCommand, Result<SignUpResponse>>
{
    private readonly IIdentityManager _identityManager;

    public SignUpCommandHandler(IIdentityManager identityManager)
    {
        _identityManager = identityManager;
    }

    public async ValueTask<Result<SignUpResponse>> Handle(SignUpCommand request,
        CancellationToken cancellationToken)
    {
        // data annotation validations
        var isInvalid = !request.TryValidate(out var errors);
        if (isInvalid)
        {
            var invalid = Result<SignUpResponse>.Invalid(errors);
            return await ValueTask.FromResult(invalid);
        }

        // registration
        var trySignUp = await _identityManager.TrySignUpAsync(request.Username,
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.EmailAddress,
            request.Password);

        if (trySignUp.IsFailure)
        {
            var failure = Result<SignUpResponse>.Inherit(result: trySignUp);
            return await ValueTask.FromResult(failure);
        }

        var userAccount = trySignUp.Value;
        var response = new SignUpResponse(userAccount.UserAccountId,
            userAccount.User.Username,
            userAccount.UserProfile.FullName,
            userAccount.UserProfile.DateOfBirth,
            userAccount.User.EmailAddress,
            userAccount.UserRoles.Select(role => role.ToString()).ToList());

        var ok = Result<SignUpResponse>.Ok(response);
        return await ValueTask.FromResult(ok);
    }
}
