using System.ComponentModel.DataAnnotations;

namespace CleanArch.Application.Identity.Commands.UserRole.Revoke;

public record RevokeUserRoleCommand : IRequest<Result>
{
    public RevokeUserRoleCommand(Guid senderAccountId, string accessPassword, Guid subjectAccountId, string role)
    {
        SenderAccountId = senderAccountId;
        SubjectAccountId = subjectAccountId;
        AccessPassword = accessPassword;
        Role = role;
    }

    [Required] public Guid SenderAccountId { get; init; }
    [Required] public Guid SubjectAccountId { get; init; }
    [Required] public string AccessPassword { get; init; }

    [Required]
    [EnumDataType(typeof(Domain.Enums.UserRole))]
    public string Role { get; init; }

}
