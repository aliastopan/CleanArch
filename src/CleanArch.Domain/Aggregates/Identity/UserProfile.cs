#nullable disable

namespace CleanArch.Domain.Aggregates.Identity;

public class UserProfile
{
    public Guid UserProfileId { get; init; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly DateOfBirth { get ; init; }
}
