using System.ComponentModel.DataAnnotations;
using CleanArch.Application.Common.Validations;

namespace CleanArch.Application.Identity.Commands.UserRole.Grant;

public record GrantUserRoleCommand : IRequest<Result>
{
    public GrantUserRoleCommand(Guid senderAccountId, string accessPassword, Guid subjectAccountId, string role)
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
    [UserRoleValidation]
    public string Role { get; init; }

}
