namespace CleanArch.Application.Identity.Commands.UserRole.Grant;

public class GrantUserRoleCommandHandler : IRequestHandler<GrantUserRoleCommand, Result>
{
    private readonly IIdentityService _identityService;

    public GrantUserRoleCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async ValueTask<Result> Handle(GrantUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        var tryGrantRole = await _identityService.TryGrantRoleAsync(request.UserAccountId, request.Role);
        if(!tryGrantRole.IsSuccess)
        {
            var failure = Result.Inherit(result: tryGrantRole);
            return await ValueTask.FromResult(failure);
        }

        var ok = Result.Ok();
        return await ValueTask.FromResult(ok);
    }
}
