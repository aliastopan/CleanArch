#nullable disable

namespace CleanArch.Domain.Entities.Identity;

public class RefreshToken
{
    public Guid RefreshTokenId { get; set; }
    public string Token { get; set; }
    public string Jti { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset ExpiryDate { get; set; }
    public bool IsUsed { get; set; }
    public bool IsInvalidated { get; set; }

    public Guid UserId { get; set; }
    public virtual User User { get; set;}
}
