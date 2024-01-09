using CleanArch.Shared.Models.Identity;

namespace CleanArch.Shared.Contracts.Identity;

public class RefreshRequest : RefreshAuthenticationModel
{
    public RefreshRequest(string accessToken, string refreshTokenStr)
    {
        base.AccessToken = accessToken;
        base.RefreshTokenStr = refreshTokenStr;
    }
}
