namespace CleanArch.Application.Identity.Commands.UserRole.Revoke;

public class RevokeUserRoleCommandHandler : IRequestHandler<RevokeUserRoleCommand, Result>
{
    private readonly IAuthenticationManager _authenticationManager;
    private readonly IIdentityManager _identityManager;

    public RevokeUserRoleCommandHandler(IAuthenticationManager authenticationManager,
        IIdentityManager identityManager)
    {
        _authenticationManager = authenticationManager;
        _identityManager = identityManager;
    }

    public async ValueTask<Result> Handle(RevokeUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        // data annotation validations
        var isValid = request.TryValidate(out var errors);
        if(!isValid)
        {
            var invalid = Result.Invalid(errors);
            return await ValueTask.FromResult(invalid);
        }

        var tryAccessPrompt = await _authenticationManager.TryAccessPromptAsync(request.AuthorityAccountId, request.AccessPassword);
        if(!tryAccessPrompt.IsSuccess)
        {
            var denied = Result.Inherit(result: tryAccessPrompt);
            return await ValueTask.FromResult(denied);
        }

        // revoke user role
        var tryRevokeRole = await _identityManager.TryRevokeRoleAsync(request.SubjectAccountId, request.Role);
        if(!tryRevokeRole.IsSuccess)
        {
            var failure = Result.Inherit(result: tryRevokeRole);
            return await ValueTask.FromResult(failure);
        }

        var ok = Result.Ok();
        return await ValueTask.FromResult(ok);
    }
}
