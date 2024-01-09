namespace CleanArch.Application.Identity.Commands.UserPrivilege.Revoke;

public class RevokePrivilegeCommandHandler : IRequestHandler<RevokePrivilegeCommand, Result>
{
    private readonly IAuthenticationManager _authenticationManager;
    private readonly IIdentityManager _identityManager;

    public RevokePrivilegeCommandHandler(IAuthenticationManager authenticationManager,
        IIdentityManager identityManager)
    {
        _authenticationManager = authenticationManager;
        _identityManager = identityManager;
    }

    public async ValueTask<Result> Handle(RevokePrivilegeCommand request,
        CancellationToken cancellationToken)
    {
        // data annotation validations
        var isInvalid = !request.TryValidate(out var errors);
        if (isInvalid)
        {
            var invalid = Result.Invalid(errors);
            return await ValueTask.FromResult(invalid);
        }

        var tryAccessPrompt = await _authenticationManager.TryAccessPromptAsync(request.AuthorityAccountId, request.AccessPassword);
        if (tryAccessPrompt.IsFailure)
        {
            var denied = Result.Inherit(result: tryAccessPrompt);
            return await ValueTask.FromResult(denied);
        }

        // revoke user privilege
        var tryRevokePrivilege = await _identityManager.TryRevokePrivilegeAsync(request.SubjectAccountId, request.Privilege);
        if (tryRevokePrivilege.IsFailure)
        {
            var failure = Result.Inherit(result: tryRevokePrivilege);
            return await ValueTask.FromResult(failure);
        }

        var ok = Result.Ok();
        return await ValueTask.FromResult(ok);
    }
}
