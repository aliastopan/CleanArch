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
        Result<LoginCommandResponse> result;

        var isValid = request.TryValidate(out var errors);
        if(!isValid)
        {
            result = Result<LoginCommandResponse>.Invalid(errors);
            return await ValueTask.FromResult(result);
        }

        var signInResult = await _userAuthenticationService.AuthenticateUserAsync(request.Username,
            request.Password);

        if(!signInResult.IsSuccess)
        {
            result = Result<LoginCommandResponse>.Inherit(result: signInResult);
            return await ValueTask.FromResult(result);
        }

        var (accessToken, refreshToken) = signInResult.Value;
        var response = new LoginCommandResponse(accessToken, refreshToken.Token);

        result = Result<LoginCommandResponse>.Ok(response);
        return await ValueTask.FromResult(result);
    }
}