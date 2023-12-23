#nullable disable

namespace CleanArch.Domain.Aggregates.Identity;

public class User
{
    public Guid UserId { get; init; }
    public string Username { get; init; }
    public string EmailAddress { get; set; }
}
