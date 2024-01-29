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
        if (tryRefreshAccess.IsFailure())
        {
            return Result<RefreshAccessResponse>.Inherit(result: tryRefreshAccess);
        }

        var (accessToken, refreshToken) = tryRefreshAccess.Value;
        var response = new RefreshAccessResponse(accessToken, refreshToken.Token);

        return Result<RefreshAccessResponse>.Ok(response);
    }
}
