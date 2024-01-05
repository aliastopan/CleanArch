using CleanArch.Domain.Aggregates.Identity;
using CleanArch.Shared.Contracts.Identity;

namespace CleanArch.Application.Identity.Commands.Authentication;

public class SignInCommandHandler : IRequestHandler<SignInCommand, Result<SignInResponse>>
{
    private readonly IAuthenticationManager _authenticationManager;

    public SignInCommandHandler(IAuthenticationManager authenticationManager)
    {
        _authenticationManager = authenticationManager;
    }

    public async ValueTask<Result<SignInResponse>> Handle(SignInCommand request,
        CancellationToken cancellationToken)
    {
        // data annotation validations
        var isInvalid = !request.TryValidate(out var errors);
        if (isInvalid)
        {
            var invalid = Result<SignInResponse>.Invalid(errors);
            return await ValueTask.FromResult(invalid);
        }

        // authentication
        var trySignIn = await _authenticationManager.TrySignInAsync(request.Username, request.Password);
        if (trySignIn.IsFailure)
        {
            var failure = Result<SignInResponse>.Inherit(result: trySignIn);
            return await ValueTask.FromResult(failure);
        }

        var (accessToken, refreshToken) = trySignIn.Value;
        var response = new SignInResponse(accessToken, refreshToken.Token);

        var ok = Result<SignInResponse>.Ok(response);
        return await ValueTask.FromResult(ok);
    }
}