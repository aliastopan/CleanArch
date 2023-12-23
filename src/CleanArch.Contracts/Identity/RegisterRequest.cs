namespace CleanArch.Contracts.Identity;

public record RegisterRequest(string Username,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    string Password);
