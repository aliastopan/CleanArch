namespace CleanArch.Contracts.Identity;

public class SignUpRequest : RegistrationModel
{
    public SignUpRequest(string username, string firstName, string lastName,
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
