using CleanArch.Shared.Contracts.Identity;

namespace CleanArch.Application.Identity.Commands.Authentication.Refresh;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, Result<RefreshResponse>>
{
    private readonly IAuthenticationManager _authenticationManager;

    public RefreshCommandHandler(IAuthenticationManager authenticationManager)
    {
        _authenticationManager = authenticationManager;
    }

    public async ValueTask<Result<RefreshResponse>> Handle(RefreshCommand request,
        CancellationToken cancellationToken)
    {
        var tryRefreshAccess = await _authenticationManager.TryRefreshAccessAsync(request.AccessToken, request.RefreshTokenStr);
        if (tryRefreshAccess.IsFailure)
        {
            var failure = Result<RefreshResponse>.Inherit(result: tryRefreshAccess);
            return await ValueTask.FromResult(failure);
        }

        var (accessToken, refreshToken) = tryRefreshAccess.Value;
        var response = new RefreshResponse(accessToken, refreshToken.Token);

        var ok = Result<RefreshResponse>.Ok(response);
        return await ValueTask.FromResult(ok);
    }
}
