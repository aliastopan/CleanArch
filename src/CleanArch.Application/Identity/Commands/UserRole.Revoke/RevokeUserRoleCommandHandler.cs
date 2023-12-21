namespace CleanArch.Application.Identity.Commands.UserRole.Revoke;

public class RevokeUserRoleCommandHandler : IRequestHandler<RevokeUserRoleCommand, Result>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IIdentityService _identityService;

    public RevokeUserRoleCommandHandler(IAuthenticationService authenticationService,
        IIdentityService identityService)
    {
        _authenticationService = authenticationService;
        _identityService = identityService;
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

        var tryAccessPrompt = await _authenticationService
            .TryAccessPromptAsync(request.SenderAccountId, request.AccessPassword);

        if(!tryAccessPrompt.IsSuccess)
        {
            var denied = Result.Inherit(result: tryAccessPrompt);
            return await ValueTask.FromResult(denied);
        }

        // revoke user role
        var tryRevokeRole = await _identityService.TryRevokeRoleAsync(request.SubjectAccountId, request.Role);
        if(!tryRevokeRole.IsSuccess)
        {
            var failure = Result.Inherit(result: tryRevokeRole);
            return await ValueTask.FromResult(failure);
        }

        var ok = Result.Ok();
        return await ValueTask.FromResult(ok);
    }
}
