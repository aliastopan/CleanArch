using CleanArch.Shared.Interfaces.Models.Identity;

namespace CleanArch.Shared.Contracts.Identity.Registration;

public record SignUpRequest(string Username, string FirstName, string LastName, DateOnly DateOfBirth, string EmailAddress, string Password)
    : IRegistrationModel;
