using System.ComponentModel.DataAnnotations;
using CleanArch.Shared.Models.Identity;

namespace CleanArch.Application.Identity.Commands.UserRole.Grant;

public class GrantUserRoleCommand : UserRoleUpdateModel, IRequest<Result>
{
    public GrantUserRoleCommand(Guid senderAccountId, string accessPassword,
        Guid subjectAccountId, string role)
    {
        base.SenderAccountId = senderAccountId;
        base.AccessPassword = accessPassword;
        base.SubjectAccountId = subjectAccountId;
        base.Role = role;
    }

    [Required]
    [EnumDataType(typeof(Domain.Enums.UserRole))]
    public new string Role
    {
        get { return base.Role; }
    }
}
