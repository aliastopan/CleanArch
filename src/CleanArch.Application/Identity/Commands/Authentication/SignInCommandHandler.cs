using CleanArch.Shared.Contracts.Identity.Authentication;

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
            return Result<SignInResponse>.Invalid(errors);
        }

        // authentication
        var trySignIn = await _authenticationManager.TrySignInAsync(request.Username, request.Password);
        if (trySignIn.IsFailure())
        {
            return Result<SignInResponse>.Inherit(result: trySignIn);
        }

        var (accessToken, refreshToken) = trySignIn.Value;
        var response = new SignInResponse(accessToken, refreshToken.Token);

        return Result<SignInResponse>.Ok(response);
    }
}