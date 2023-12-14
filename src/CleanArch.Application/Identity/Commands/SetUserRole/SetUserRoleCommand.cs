namespace CleanArch.Application.Identity.Commands.SetUserRole;

public record SetUserRoleCommand(Guid GrantorId,
    string PermissionPassword,
    Guid GranteeId,
    int Role) : IRequest<Result>;
