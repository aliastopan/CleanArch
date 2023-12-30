namespace CleanArch.Application.Identity.Commands.Authentication;

public class SignInCommandHandler : IRequestHandler<SignInCommand, Result<SignInCommandResponse>>
{
    private readonly IAuthenticationManager _authenticationManager;

    public SignInCommandHandler(IAuthenticationManager authenticationManager)
    {
        _authenticationManager = authenticationManager;
    }

    public async ValueTask<Result<SignInCommandResponse>> Handle(SignInCommand request,
        CancellationToken cancellationToken)
    {
        // data annotation validations
        var isValid = request.TryValidate(out var errors);
        if(!isValid)
        {
            var invalid = Result<SignInCommandResponse>.Invalid(errors);
            return await ValueTask.FromResult(invalid);
        }

        // authentication
        var trySignIn = await _authenticationManager.TrySignInAsync(request.Username, request.Password);
        if(!trySignIn.IsSuccess)
        {
            var failure = Result<SignInCommandResponse>.Inherit(result: trySignIn);
            return await ValueTask.FromResult(failure);
        }

        var (accessToken, refreshToken) = trySignIn.Value;
        var response = new SignInCommandResponse(accessToken, refreshToken.Token);

        var ok = Result<SignInCommandResponse>.Ok(response);
        return await ValueTask.FromResult(ok);
    }
}