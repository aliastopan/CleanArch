using CleanArch.Domain.Entities.Identity;

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

        var validateRefreshToken = _securityTokenService.ValidateRefreshToken(request.AccessToken, request.RefreshToken);
        if(!validateRefreshToken.IsSuccess)
        {
            result = Result<RefreshCommandResponse>.Inherit(result: validateRefreshToken);
            return await ValueTask.FromResult(result);
        }

        var userAccount = validateRefreshToken.Value.UserAccount;
        var accessToken = _securityTokenService.GenerateAccessToken(userAccount);
        var refreshToken = _securityTokenService.GenerateRefreshToken(accessToken, userAccount);

        using var dbContext = _dbContextFactory.CreateDbContext();
        RefreshToken previousRefreshToken = validateRefreshToken.Value;
        RefreshToken currentRefreshToken = refreshToken.Value;

        dbContext.RefreshTokens.Update(previousRefreshToken);
        dbContext.RefreshTokens.Add(currentRefreshToken);
        await dbContext.SaveChangesAsync();

        var response = new RefreshCommandResponse(accessToken, currentRefreshToken.Token);
        result = Result<RefreshCommandResponse>.Ok(response);
        return await ValueTask.FromResult(result);
    }
}
