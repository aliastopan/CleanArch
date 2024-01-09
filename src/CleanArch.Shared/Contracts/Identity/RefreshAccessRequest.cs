using CleanArch.Shared.Models.Identity;

namespace CleanArch.Shared.Contracts.Identity;

public class RefreshAccessRequest : RefreshAuthenticationModel
{
    public RefreshAccessRequest(string accessToken, string refreshTokenStr)
    {
        base.AccessToken = accessToken;
        base.RefreshTokenStr = refreshTokenStr;
    }
}
