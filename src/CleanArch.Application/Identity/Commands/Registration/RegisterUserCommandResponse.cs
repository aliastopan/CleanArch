namespace CleanArch.Application.Identity.Commands.Registration;

public record RegisterUserCommandResponse(Guid UserAccountId,
    string Username,
    string Email,
    string UserRole);
