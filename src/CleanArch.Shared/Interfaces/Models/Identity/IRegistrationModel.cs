namespace CleanArch.Shared.Interfaces.Models.Identity;

public interface IRegistrationModel
{
   string Username { get; }
   string FirstName { get; }
   string LastName { get; }
   DateOnly DateOfBirth { get; }
   string EmailAddress { get; }
   string Password { get; }
}
