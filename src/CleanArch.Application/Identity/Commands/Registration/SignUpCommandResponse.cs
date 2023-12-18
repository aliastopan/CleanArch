namespace CleanArch.Application.Identity.Commands.Registration;

public record SignUpCommandResponse(Guid UserAccountId,
    string Username,
    string Email,
    string UserRole);
