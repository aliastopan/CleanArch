#nullable disable
using CleanArch.Domain.Enums;

namespace CleanArch.Domain.Entities.Identity;

public class User
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
    public bool IsVerified { get; set; }
    public string HashedPassword { get; set; }
    public string Salt { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset LastLoggedIn { get; set; }
}
