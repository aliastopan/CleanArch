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
            return Result.Invalid(errors);
        }

        // validate access
        var tryAccessPrompt = await _authenticationManager.TryAccessPromptAsync(request.AuthorityAccountId, request.AccessPassword);
        if (tryAccessPrompt.IsFailure())
        {
            return Result.Inherit(result: tryAccessPrompt);
        }

        // set role
        var trySetRole = await _identityManager.TrySetRoleAsync(request.SubjectAccountId, request.Role);
        if (trySetRole.IsFailure())
        {
            return Result.Inherit(result: trySetRole);
        }

        return Result.Ok();
    }
}
