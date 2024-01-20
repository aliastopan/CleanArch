using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Infrastructure.Managers;

internal sealed class AuthenticationManager : IAuthenticationManager
{
    private readonly IIdentityAggregateHandler _identityAggregateHandler;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthenticationManager(IIdentityAggregateHandler identityAggregateHandler,
        IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService)
    {
        _identityAggregateHandler = identityAggregateHandler;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result<(string accessToken, RefreshToken refreshToken)>> TrySignInAsync(string username, string password)
    {
        var tryGetUserAccount = await _identityAggregateHandler.TryGetUserAccountAsync(username);
        if (tryGetUserAccount.IsFailure())
        {
            return Result<(string, RefreshToken)>.Inherit(result: tryGetUserAccount);
        }

        var userAccount = tryGetUserAccount.Value;

        var tryValidatePassword = _identityAggregateHandler.TryValidatePassword(password, userAccount.PasswordSalt, userAccount.PasswordHash);
        if (tryValidatePassword.IsFailure())
        {
            return Result<(string, RefreshToken)>.Inherit(result: tryValidatePassword);
        }

        // sign-in protocol
        var accessToken = _accessTokenService.GenerateAccessToken(userAccount);
        var tryGetRefreshToken = _refreshTokenService.TryGenerateRefreshToken(accessToken, userAccount);
        if (tryGetRefreshToken.IsFailure())
        {
            return Result<(string, RefreshToken)>.Inherit(result: tryGetRefreshToken);
        }

        var refreshToken = tryGetRefreshToken.Value;
        await _identityAggregateHandler.SignUserAsync(userAccount, refreshToken);

        return Result<(string, RefreshToken)>.Ok((accessToken, refreshToken));
    }

    public async Task<Result<(string accessToken, RefreshToken refreshToken)>> TryRefreshAccessAsync(string accessToken, string refreshTokenStr)
    {
        var tryValidateSecurityToken = _refreshTokenService.TryValidateSecurityToken(accessToken, refreshTokenStr);
        if (tryValidateSecurityToken.IsFailure())
        {
            return Result<(string, RefreshToken)>.Inherit(result: tryValidateSecurityToken);
        }

        var userAccount = tryValidateSecurityToken.Value.UserAccount;
        accessToken = _accessTokenService.GenerateAccessToken(userAccount);
        var tryGetRefreshToken = _refreshTokenService.TryGenerateRefreshToken(accessToken, userAccount);
        if (tryGetRefreshToken.IsFailure())
        {
            return Result<(string, RefreshToken)>.Inherit(result: tryGetRefreshToken);
        }

        var previousRefreshToken = tryValidateSecurityToken.Value;
        var currentRefreshToken = tryGetRefreshToken.Value;
        await _identityAggregateHandler.RotateRefreshTokenAsync(previousRefreshToken, currentRefreshToken);

        return Result<(string, RefreshToken)>.Ok((accessToken, currentRefreshToken));
    }

    public async Task<Result> TryAccessPromptAsync(Guid userAccountId, string password)
    {
        var tryGetUserAccount = await _identityAggregateHandler.TryGetUserAccountAsync(userAccountId);
        if (tryGetUserAccount.IsFailure())
        {
            return Result.Inherit(result: tryGetUserAccount);
        }

        var userAccount = tryGetUserAccount.Value;

        var validatePassword = _identityAggregateHandler.TryValidatePassword(password, userAccount.PasswordSalt, userAccount.PasswordHash);
        if (validatePassword.IsFailure())
        {
            return Result.Inherit(result: validatePassword);
        }

        return Result.Ok();
    }
}
