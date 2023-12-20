namespace CleanArch.Contracts.Identity;

public record GrantUserRoleRequest(Guid SenderAccountId,
    string AccessPassword,
    Guid RecipientAccountId,
    string Role);
