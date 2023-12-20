namespace CleanArch.Application.Identity.Commands.UserRole.Grant;

public record GrantUserRoleCommand(Guid UserAccountId, string Role) : IRequest<Result>;
