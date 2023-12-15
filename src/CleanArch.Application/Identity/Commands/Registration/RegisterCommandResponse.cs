namespace CleanArch.Application.Identity.Commands.Registration;

public record RegisterCommandResponse(Guid UserAccountId,
    string Username,
    string Email,
    string UserRole);
