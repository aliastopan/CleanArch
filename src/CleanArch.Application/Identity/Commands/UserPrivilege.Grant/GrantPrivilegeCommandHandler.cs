namespace CleanArch.Application.Identity.Commands.UserPrivilege;

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
            var invalid = Result.Invalid(errors);
            return await ValueTask.FromResult(invalid);
        }

        var tryAccessPrompt = await _authenticationManager.TryAccessPromptAsync(request.AuthorityAccountId, request.AccessPassword);
        if (tryAccessPrompt.IsFailure)
        {
            var denied = Result.Inherit(result: tryAccessPrompt);
            return await ValueTask.FromResult(denied);
        }

        // grant privilege
        var tryGrantPrivilege = await _identityManager.TryGrantPrivilegeAsync(request.SubjectAccountId, request.Privilege);
        if (tryGrantPrivilege.IsFailure)
        {
            var failure = Result.Inherit(result: tryGrantPrivilege);
            return await ValueTask.FromResult(failure);
        }

        var ok = Result.Ok();
        return await ValueTask.FromResult(ok);
    }
}
