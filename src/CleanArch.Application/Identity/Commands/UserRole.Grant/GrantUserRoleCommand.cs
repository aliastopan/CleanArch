using System.ComponentModel.DataAnnotations;
using CleanArch.Application.Common.Validations;

namespace CleanArch.Application.Identity.Commands.UserRole.Grant;

public record GrantUserRoleCommand : IRequest<Result>
{
    public GrantUserRoleCommand(Guid userAccountId, string role)
    {
        UserAccountId = userAccountId;
        Role = role;
    }

    [Required]
    public Guid UserAccountId { get; init; }

    [Required]
    [UserRoleValidation]
    public string Role { get; init; }
}
