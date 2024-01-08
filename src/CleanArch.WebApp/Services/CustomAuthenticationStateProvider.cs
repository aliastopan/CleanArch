using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using CleanArch.Application.Common.Interfaces.Services;

namespace CleanArch.WebApp.Services;

public sealed class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IAccessTokenService _accessTokenService;
    private readonly ProtectedSessionStorage _sessionStorage;

    public CustomAuthenticationStateProvider(IAccessTokenService accessTokenService,
        ProtectedSessionStorage sessionStorage)
    {
        _accessTokenService = accessTokenService;
        _sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var tryAuth = await TryAuthenticateAsync();

        if (tryAuth.IsFailure)
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }

        var accessToken = tryAuth.Value;
        var principal = _accessTokenService.GetPrincipalFromToken(accessToken);
        var authenticationState = new AuthenticationState(principal);

        NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
        return authenticationState;
    }

    private async Task<Result<string>> TryAuthenticateAsync()
    {
        var result = await _sessionStorage.GetAsync<string>("access-token");

        if (result.Success)
        {
            var accessToken = result.Value;
            return Result<string>.Ok(accessToken!);
        }

        return Result<string>.Error();
    }
}
