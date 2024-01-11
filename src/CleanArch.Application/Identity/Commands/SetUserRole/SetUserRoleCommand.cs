using System.ComponentModel.DataAnnotations;
using CleanArch.Shared.Models.Identity;

namespace CleanArch.Application.Identity.Commands.SetUserRole;

public class SetUserRoleCommand : UserRoleUpdateModel, IRequest<Result>
{
    public SetUserRoleCommand(Guid authorityAccountId, string accessPassword,
        Guid subjectAccountId, string role)
    {
        base.AuthorityAccountId = authorityAccountId;
        base.AccessPassword = accessPassword;
        base.SubjectAccountId = subjectAccountId;
        base.Role = role;
    }

    [Required]
    [EnumDataType(typeof(Domain.Enums.UserPrivilege))]
    public new string Role
    {
        get { return base.Role; }
    }
}
