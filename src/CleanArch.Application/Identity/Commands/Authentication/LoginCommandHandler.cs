namespace CleanArch.Application.Identity.Commands.Authentication;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginCommandResponse>>
{
    private readonly IUserAuthenticationService _userAuthenticationService;

    public LoginCommandHandler(IUserAuthenticationService userAuthenticationService)
    {
        _userAuthenticationService = userAuthenticationService;
    }

    public async ValueTask<Result<LoginCommandResponse>> Handle(LoginCommand request,
        CancellationToken cancellationToken)
    {
        var isValid = request.TryValidate(out var errors);
        if(!isValid)
        {
            var invalid = Result<LoginCommandResponse>.Invalid(errors);
            return await ValueTask.FromResult(invalid);
        }

        var signInResult = await _userAuthenticationService.AuthenticateUserAsync(request.Username,
            request.Password);

        if(!signInResult.IsSuccess)
        {
            var failure = Result<LoginCommandResponse>.Inherit(result: signInResult);
            return await ValueTask.FromResult(failure);
        }

        var (accessToken, refreshToken) = signInResult.Value;
        var response = new LoginCommandResponse(accessToken, refreshToken.Token);

        var ok = Result<LoginCommandResponse>.Ok(response);
        return await ValueTask.FromResult(ok);
    }
}