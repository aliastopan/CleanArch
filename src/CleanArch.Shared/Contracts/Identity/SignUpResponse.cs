namespace CleanArch.Shared.Contracts.Identity;

public record SignUpResponse(Guid UserAccountId,
    string Username,
    string FullName,
    DateOnly DateOfBirth,
    string EmailAddress,
    List<string> UserRoles);