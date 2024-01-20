namespace CleanArch.Application.Identity.Commands.SetUserRole;

public class SetUserRoleCommandHandler : IRequestHandler<SetUserRoleCommand, Result>
{
    private readonly IAuthenticationManager _authenticationManager;
    private readonly IIdentityManager _identityManager;

    public SetUserRoleCommandHandler(IAuthenticationManager authenticationManager,
        IIdentityManager identityManager)
    {
        _authenticationManager = authenticationManager;
        _identityManager = identityManager;
    }

    public async ValueTask<Result> Handle(SetUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        // data annotation validations
        var isInvalid = !request.TryValidate(out var errors);
        if (isInvalid)
        {
            var invalid = Result.Invalid(errors);
            return await ValueTask.FromResult(invalid);
        }

        // validate access
        var tryAccessPrompt = await _authenticationManager.TryAccessPromptAsync(request.AuthorityAccountId, request.AccessPassword);
        if (tryAccessPrompt.IsFailure())
        {
            var denied = Result.Inherit(result: tryAccessPrompt);
            return await ValueTask.FromResult(denied);
        }

        // set role
        var trySetRole = await _identityManager.TrySetRoleAsync(request.SubjectAccountId, request.Role);
        if (trySetRole.IsFailure())
        {
            var failure = Result.Inherit(result: trySetRole);
            return await ValueTask.FromResult(failure);
        }

        var ok = Result.Ok();
        return await ValueTask.FromResult(ok);
    }
}
