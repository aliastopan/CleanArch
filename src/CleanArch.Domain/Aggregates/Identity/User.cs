#nullable disable

namespace CleanArch.Domain.Aggregates.Identity;

public class User
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}
