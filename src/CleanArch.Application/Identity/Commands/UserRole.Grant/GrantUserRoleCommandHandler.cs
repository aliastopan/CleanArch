namespace CleanArch.Application.Identity.Commands.UserRole.Grant;

public class GrantUserRoleCommandHandler : IRequestHandler<GrantUserRoleCommand, Result>
{
    private readonly IAuthenticationManager _authenticationManager;
    private readonly IIdentityManager _identityManager;

    public GrantUserRoleCommandHandler(IAuthenticationManager authenticationManager,
        IIdentityManager identityManager)
    {
        _authenticationManager = authenticationManager;
        _identityManager = identityManager;
    }

    public async ValueTask<Result> Handle(GrantUserRoleCommand request,
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

        // grant user role
        var tryGrantRole = await _identityManager.TryGrantRoleAsync(request.SubjectAccountId, request.Role);
        if(!tryGrantRole.IsSuccess)
        {
            var failure = Result.Inherit(result: tryGrantRole);
            return await ValueTask.FromResult(failure);
        }

        var ok = Result.Ok();
        return await ValueTask.FromResult(ok);
    }
}
