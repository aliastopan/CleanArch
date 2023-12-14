namespace CleanArch.Contracts.Identity;

public record SetRoleRequest(Guid GrantorId,
    string PermissionPassword,
    Guid GranteeId,
    int Role);
