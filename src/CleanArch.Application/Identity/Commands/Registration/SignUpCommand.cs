using CleanArch.Shared.Models.Identity;

namespace CleanArch.Application.Identity.Commands.Registration;

public class SignUpCommand : RegistrationModel, IRequest<Result<SignUpCommandResponse>>
{
    public SignUpCommand(string username, string firstName, string lastName,
        DateOnly dateOfBirth, string emailAddress, string password)
    {
        base.Username = username;
        base.FirstName = firstName;
        base.LastName = lastName;
        base.DateOfBirth = dateOfBirth;
        base.EmailAddress = emailAddress;
        base.Password = password;
    }
}
