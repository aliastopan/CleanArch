namespace CleanArch.Contracts.Identity;

public record RevokeUserRoleRequest(Guid SenderAccountId,
    string AccessPassword,
    Guid RecipientAccountId,
    string Role);
