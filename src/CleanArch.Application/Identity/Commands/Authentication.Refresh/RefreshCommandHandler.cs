namespace CleanArch.Application.Identity.Commands.Authentication.Refresh;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, Result<RefreshCommandResponse>>
{
    private readonly IAuthenticationManager _authenticationManager;

    public RefreshCommandHandler(IAuthenticationManager authenticationManager)
    {
        _authenticationManager = authenticationManager;
    }

    public async ValueTask<Result<RefreshCommandResponse>> Handle(RefreshCommand request,
        CancellationToken cancellationToken)
    {
        var tryRefreshAccess = await _authenticationManager.TryRefreshAccessAsync(request.AccessToken, request.RefreshToken);
        if(!tryRefreshAccess.IsSuccess)
        {
            var failure = Result<RefreshCommandResponse>.Inherit(result: tryRefreshAccess);
            return await ValueTask.FromResult(failure);
        }

        var (accessToken, refreshToken) = tryRefreshAccess.Value;
        var response = new RefreshCommandResponse(accessToken, refreshToken.Token);

        var ok = Result<RefreshCommandResponse>.Ok(response);
        return await ValueTask.FromResult(ok);
    }
}
