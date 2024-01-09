using CleanArch.Shared.Contracts.Identity;
using CleanArch.Shared.Models.Identity;

namespace CleanArch.Application.Identity.Commands.Authentication.Refresh;

public class RefreshCommand : RefreshAuthenticationModel, IRequest<Result<RefreshResponse>>
{
    public RefreshCommand(string accessToken, string refreshTokenStr)
    {
        base.AccessToken = accessToken;
        base.RefreshTokenStr = refreshTokenStr;
    }
}
