namespace CleanArch.Application.Identity.Commands.GrantUserPrivilege;

public class GrantPrivilegeCommandHandler : IRequestHandler<GrantPrivilegeCommand, Result>
{
    private readonly IAuthenticationManager _authenticationManager;
    private readonly IIdentityManager _identityManager;

    public GrantPrivilegeCommandHandler(IAuthenticationManager authenticationManager,
        IIdentityManager identityManager)
    {
        _authenticationManager = authenticationManager;
        _identityManager = identityManager;
    }

    public async ValueTask<Result> Handle(GrantPrivilegeCommand request,
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

        // grant privilege
        var tryGrantPrivilege = await _identityManager.TryGrantPrivilegeAsync(request.SubjectAccountId, request.Privilege);
        if (tryGrantPrivilege.IsFailure())
        {
            return Result.Inherit(result: tryGrantPrivilege);
        }

        return Result.Ok();
    }
}
