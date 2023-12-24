namespace CleanArch.Contracts.Identity;

public record RevokeUserRoleRequest(Guid AuthorityAccountId,
    string AccessPassword,
    Guid SubjectAccountId,
    string Role);
