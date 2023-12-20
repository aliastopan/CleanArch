namespace CleanArch.Contracts.Identity;

public record GrantUserRoleRequest(Guid UserAccountId, string Role);
