#nullable disable
using CleanArch.Domain.Enums;

namespace CleanArch.Domain.Entities.Identity;

public class User
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}
