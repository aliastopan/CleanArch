using System.ComponentModel.DataAnnotations;
using CleanArch.Application.Common.Validations;

namespace CleanArch.Application.Identity.Commands.UserRole.Revoke;

public record RevokeUserRoleCommand : IRequest<Result>
{
    public RevokeUserRoleCommand(Guid senderAccountId, string accessPassword, Guid recipientAccountId, string role)
    {
        SenderAccountId = senderAccountId;
        RecipientAccountId = recipientAccountId;
        AccessPassword = accessPassword;
        Role = role;
    }

    [Required] public Guid SenderAccountId { get; init; }
    [Required] public Guid RecipientAccountId { get; init; }
    [Required] public string AccessPassword { get; init; }

    [Required]
    [UserRoleValidation]
    public string Role { get; init; }

}
