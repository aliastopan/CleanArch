using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Application.Identity.Commands.Authentication.Refresh;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, Result<RefreshCommandResponse>>
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;
    private readonly ISecurityTokenService _securityTokenService;

    public RefreshCommandHandler(IAppDbContextFactory<IAppDbContext> dbContextFactory,
        ISecurityTokenService securityTokenService)
    {
        _dbContextFactory = dbContextFactory;
        _securityTokenService = securityTokenService;
    }

    public async ValueTask<Result<RefreshCommandResponse>> Handle(RefreshCommand request,
        CancellationToken cancellationToken)
    {
        Result<RefreshCommandResponse> result;

        var validateSecurityToken = _securityTokenService.TryValidateSecurityToken(request.AccessToken, request.RefreshToken);
        if(!validateSecurityToken.IsSuccess)
        {
            result = Result<RefreshCommandResponse>.Inherit(result: validateSecurityToken);
            return await ValueTask.FromResult(result);
        }

        var userAccount = validateSecurityToken.Value.UserAccount;
        var accessToken = _securityTokenService.GenerateAccessToken(userAccount);
        var refreshToken = _securityTokenService.TryGenerateRefreshToken(accessToken, userAccount);

        using var dbContext = _dbContextFactory.CreateDbContext();
        RefreshToken previousRefreshToken = validateSecurityToken.Value;
        RefreshToken currentRefreshToken = refreshToken.Value;

        dbContext.RefreshTokens.Update(previousRefreshToken);
        dbContext.RefreshTokens.Add(currentRefreshToken);
        await dbContext.SaveChangesAsync();

        var response = new RefreshCommandResponse(accessToken, currentRefreshToken.Token);
        result = Result<RefreshCommandResponse>.Ok(response);
        return await ValueTask.FromResult(result);
    }
}
