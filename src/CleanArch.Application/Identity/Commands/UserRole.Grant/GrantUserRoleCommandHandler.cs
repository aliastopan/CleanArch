namespace CleanArch.Application.Identity.Commands.UserRole.Grant;

public class GrantUserRoleCommandHandler : IRequestHandler<GrantUserRoleCommand, Result>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IIdentityService _identityService;

    public GrantUserRoleCommandHandler(IAuthenticationService authenticationService,
        IIdentityService identityService)
    {
        _authenticationService = authenticationService;
        _identityService = identityService;
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

        var tryAccessPrompt = await _authenticationService.TryAccessPromptAsync(request.SenderAccountId, request.AccessPassword);
        if(!tryAccessPrompt.IsSuccess)
        {
            var denied = Result.Inherit(result: tryAccessPrompt);
            return await ValueTask.FromResult(denied);
        }

        // grant user role
        var tryGrantRole = await _identityService.TryGrantRoleAsync(request.SubjectAccountId, request.Role);
        if(!tryGrantRole.IsSuccess)
        {
            var failure = Result.Inherit(result: tryGrantRole);
            return await ValueTask.FromResult(failure);
        }

        var ok = Result.Ok();
        return await ValueTask.FromResult(ok);
    }
}
