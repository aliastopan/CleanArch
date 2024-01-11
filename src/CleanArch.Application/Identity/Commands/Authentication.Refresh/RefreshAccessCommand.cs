using CleanArch.Shared.Contracts.Identity.Authentication;
using CleanArch.Shared.Models.Identity;

namespace CleanArch.Application.Identity.Commands.Authentication;

public class RefreshAccessCommand : RefreshAuthenticationModel, IRequest<Result<RefreshAccessResponse>>
{
    public RefreshAccessCommand(string accessToken, string refreshTokenStr)
    {
        base.AccessToken = accessToken;
        base.RefreshTokenStr = refreshTokenStr;
    }
}
