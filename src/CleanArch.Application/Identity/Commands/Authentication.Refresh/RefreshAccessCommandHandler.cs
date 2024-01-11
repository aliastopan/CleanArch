using CleanArch.Shared.Contracts.Identity.Authentication;

namespace CleanArch.Application.Identity.Commands.Authentication;

public class RefreshAccessCommandHandler : IRequestHandler<RefreshAccessCommand, Result<RefreshAccessResponse>>
{
    private readonly IAuthenticationManager _authenticationManager;

    public RefreshAccessCommandHandler(IAuthenticationManager authenticationManager)
    {
        _authenticationManager = authenticationManager;
    }

    public async ValueTask<Result<RefreshAccessResponse>> Handle(RefreshAccessCommand request,
        CancellationToken cancellationToken)
    {
        var tryRefreshAccess = await _authenticationManager.TryRefreshAccessAsync(request.AccessToken, request.RefreshTokenStr);
        if (tryRefreshAccess.IsFailure)
        {
            var failure = Result<RefreshAccessResponse>.Inherit(result: tryRefreshAccess);
            return await ValueTask.FromResult(failure);
        }

        var (accessToken, refreshToken) = tryRefreshAccess.Value;
        var response = new RefreshAccessResponse(accessToken, refreshToken.Token);

        var ok = Result<RefreshAccessResponse>.Ok(response);
        return await ValueTask.FromResult(ok);
    }
}
