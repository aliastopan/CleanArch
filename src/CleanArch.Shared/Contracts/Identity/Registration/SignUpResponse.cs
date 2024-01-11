namespace CleanArch.Shared.Contracts.Identity.Registration;

public record SignUpResponse(Guid UserAccountId,
    string Username,
    string FullName,
    DateOnly DateOfBirth,
    string EmailAddress,
    List<string> UserPrivileges);