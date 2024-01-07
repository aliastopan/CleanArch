using Microsoft.Extensions.Options;
using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Infrastructure.Services;

internal sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IDateTimeService _dateTimeService;
    private readonly SecurityTokenSettings _securityTokenSettings;

    public RefreshTokenService(IAppDbContextFactory<IAppDbContext> dbContextFactory,
        IAccessTokenService accessTokenService,
        IDateTimeService dateTimeService,
        IOptions<SecurityTokenSettings> securityTokenSettings)
    {
        _dbContextFactory = dbContextFactory;
        _accessTokenService = accessTokenService;
        _dateTimeService = dateTimeService;
        _securityTokenSettings = securityTokenSettings.Value;
    }

    public Result<RefreshToken> TryGenerateRefreshToken(string accessToken, UserAccount userAccount)
    {
        var principal = _accessTokenService.GetPrincipalFromToken(accessToken);
        if (principal is null)
        {
            var error = new Error("Refresh token has invalid null principal.", ErrorSeverity.Warning);
            return Result<RefreshToken>.Unauthorized(error);
        }

        var jti = principal.Claims.Single(x => x.Type == JwtClaimTypes.Jti).Value;
        var refreshToken = new RefreshToken
        {
            RefreshTokenId = Guid.NewGuid(),
            Token = Guid.NewGuid().ToString(),
            Jti = jti,
            CreationDate = _dateTimeService.DateTimeOffsetNow,
            ExpiryDate = _dateTimeService.DateTimeOffsetNow.Add(_securityTokenSettings.RefreshTokenLifeTime),
            FkUserAccountId = userAccount.UserAccountId,
            UserAccount = userAccount,
        };

        return Result<RefreshToken>.Ok(refreshToken);
    }

    public Result<RefreshToken> TryValidateSecurityToken(string accessToken, string refreshTokenStr)
    {
        var principal = _accessTokenService.GetPrincipalFromToken(accessToken);
        if (principal is null)
        {
            var error = new Error("Refresh token has invalid null principal.", ErrorSeverity.Warning);
            return Result<RefreshToken>.Unauthorized(error);
        }

        RefreshToken? currentRefreshToken;
        using (var dbContext = _dbContextFactory.CreateDbContext())
        {
            currentRefreshToken = dbContext.GetRefreshToken(token: refreshTokenStr);
        }

        if (currentRefreshToken is null)
        {
            var error = new Error("Refresh token not found.", ErrorSeverity.Warning);
            return Result<RefreshToken>.NotFound(error);
        }

        //TODO: validate every condition and stack the error result as array

        if (currentRefreshToken.ExpiryDate < _dateTimeService.UtcNow)
        {
            var error = new Error("Refresh token was expired.", ErrorSeverity.Warning);
            return Result<RefreshToken>.Unauthorized(error);
        }

        if (currentRefreshToken.IsInvalidated)
        {
            var error = new Error("Refresh token was invalidated.", ErrorSeverity.Warning);
            return Result<RefreshToken>.Invalid(error);
        }

        if (currentRefreshToken.IsUsed)
        {
            var error = new Error("Refresh token was used.", ErrorSeverity.Warning);
            return Result<RefreshToken>.Error(error);
        }

        currentRefreshToken.IsUsed = true;
        return Result<RefreshToken>.Ok(currentRefreshToken);
    }
}
