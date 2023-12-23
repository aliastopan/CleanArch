namespace CleanArch.Application.Identity.Commands.Registration;

public record SignUpCommandResponse(Guid UserAccountId,
    string Username,
    string FullName,
    DateOnly DateOfBirth,
    string Email,
    List<string> UserRoles);
