#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CleanArch.Shared.Models.Identity;

public class UserRoleUpdateModel
{
    [Required]
    public Guid AuthorityAccountId { get; init; }

    [Required]
    public Guid SubjectAccountId { get; init; }

    [Required]
    public string AccessPassword { get; init; }

    [Required]
    public string Role { get; init; }
}
