namespace CleanArch.Application.Identity.Commands.RevokeUserPrivilege;

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
            return Result.Invalid(errors);
        }

        var tryAccessPrompt = await _authenticationManager.TryAccessPromptAsync(request.AuthorityAccountId, request.AccessPassword);
        if (tryAccessPrompt.IsFailure())
        {
            return Result.Inherit(result: tryAccessPrompt);
        }

        // revoke user privilege
        var tryRevokePrivilege = await _identityManager.TryRevokePrivilegeAsync(request.SubjectAccountId, request.Privilege);
        if (tryRevokePrivilege.IsFailure())
        {
            return Result.Inherit(result: tryRevokePrivilege);
        }

        return Result.Ok();
    }
}
