using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Infrastructure.Managers;

internal sealed class AuthenticationManager : IAuthenticationManager
{
    private readonly IIdentityAggregateService _identityAggregateService;
    private readonly ISecurityTokenService _securityTokenService;

    public AuthenticationManager(IIdentityAggregateService identityAggregateService,
        ISecurityTokenService securityTokenService)
    {
        _identityAggregateService = identityAggregateService;
        _securityTokenService = securityTokenService;
    }

    public async Task<Result<(string accessToken, RefreshToken refreshToken)>> TrySignInAsync(string username, string password)
    {
        var tryGetUserAccount = await _identityAggregateService.TryGetUserAccountAsync(username);
        if(!tryGetUserAccount.IsSuccess)
        {
            return Result<(string, RefreshToken)>.Inherit(result: tryGetUserAccount);
        }

        var userAccount = tryGetUserAccount.Value;

        var validatePassword = _identityAggregateService.TryValidatePassword(password, userAccount.PasswordSalt, userAccount.PasswordHash);
        if(!validatePassword.IsSuccess)
        {
            return Result<(string, RefreshToken)>.Inherit(result: validatePassword);
        }

        // sign-in protocol
        var accessToken = _securityTokenService.GenerateAccessToken(userAccount);
        var tryGetRefreshToken = _securityTokenService.TryGenerateRefreshToken(accessToken, userAccount);
        if(!tryGetRefreshToken.IsSuccess)
        {
            return Result<(string, RefreshToken)>.Inherit(result: tryGetRefreshToken);
        }

        var refreshToken = tryGetRefreshToken.Value;
        await _identityAggregateService.SignUserAsync(userAccount, refreshToken);

        return Result<(string, RefreshToken)>.Ok((accessToken, refreshToken));
    }

    public async Task<Result<(string accessToken, RefreshToken refreshToken)>> TryRefreshAccessAsync(string accessToken, string refreshToken)
    {
        var tryValidateSecurityToken = _securityTokenService.TryValidateSecurityToken(accessToken, refreshToken);
        if(!tryValidateSecurityToken.IsSuccess)
        {
            return Result<(string, RefreshToken)>.Inherit(result: tryValidateSecurityToken);
        }

        var userAccount = tryValidateSecurityToken.Value.UserAccount;
        accessToken = _securityTokenService.GenerateAccessToken(userAccount);
        var tryGetRefreshToken = _securityTokenService.TryGenerateRefreshToken(accessToken, userAccount);
        if(!tryGetRefreshToken.IsSuccess)
        {
            return Result<(string, RefreshToken)>.Inherit(result: tryGetRefreshToken);
        }

        var previousRefreshToken = tryValidateSecurityToken.Value;
        var currentRefreshToken = tryGetRefreshToken.Value;
        await _identityAggregateService.RotateRefreshTokenAsync(previousRefreshToken, currentRefreshToken);

        return Result<(string, RefreshToken)>.Ok((accessToken, currentRefreshToken));
    }

    public async Task<Result> TryAccessPromptAsync(Guid userAccountId, string password)
    {
        var tryGetUserAccount = await _identityAggregateService.TryGetUserAccountAsync(userAccountId);
        if(!tryGetUserAccount.IsSuccess)
        {
            return Result.Inherit(result: tryGetUserAccount);
        }

        var userAccount = tryGetUserAccount.Value;

        var validatePassword = _identityAggregateService.TryValidatePassword(password, userAccount.PasswordSalt, userAccount.PasswordHash);
        if(!validatePassword.IsSuccess)
        {
            return Result.Inherit(result: validatePassword);
        }

        return Result.Ok();
    }
}
