namespace CleanArch.Application.Identity.Commands.Authentication;

public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, Result<AuthenticateUserCommandResponse>>
{
    private readonly IUserAuthenticationService _userAuthenticationService;

    public AuthenticateUserCommandHandler(IUserAuthenticationService userAuthenticationService)
    {
        _userAuthenticationService = userAuthenticationService;
    }

    public async ValueTask<Result<AuthenticateUserCommandResponse>> Handle(AuthenticateUserCommand request,
        CancellationToken cancellationToken)
    {
        var isValid = request.TryValidate(out var errors);
        if(!isValid)
        {
            var invalid = Result<AuthenticateUserCommandResponse>.Invalid(errors);
            return await ValueTask.FromResult(invalid);
        }

        var tryAuthenticateUser = await _userAuthenticationService.TryAuthenticateUserAsync(request.Username,
            request.Password);

        if(!tryAuthenticateUser.IsSuccess)
        {
            var failure = Result<AuthenticateUserCommandResponse>.Inherit(result: tryAuthenticateUser);
            return await ValueTask.FromResult(failure);
        }

        var (accessToken, refreshToken) = tryAuthenticateUser.Value;
        var response = new AuthenticateUserCommandResponse(accessToken, refreshToken.Token);

        var ok = Result<AuthenticateUserCommandResponse>.Ok(response);
        return await ValueTask.FromResult(ok);
    }
}