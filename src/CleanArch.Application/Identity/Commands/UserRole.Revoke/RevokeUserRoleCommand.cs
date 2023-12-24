using System.ComponentModel.DataAnnotations;
using CleanArch.Shared.Models.Identity;

namespace CleanArch.Application.Identity.Commands.UserRole.Revoke;

public class RevokeUserRoleCommand : UserRoleUpdateModel, IRequest<Result>
{
    public RevokeUserRoleCommand(Guid authorityAccountId, string accessPassword,
        Guid subjectAccountId, string role)
    {
        base.AuthorityAccountId = authorityAccountId;
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
