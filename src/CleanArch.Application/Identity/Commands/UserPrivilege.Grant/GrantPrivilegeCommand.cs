using System.ComponentModel.DataAnnotations;
using CleanArch.Shared.Models.Identity;

namespace CleanArch.Application.Identity.Commands.UserPrivilege;

public class GrantPrivilegeCommand : UserPrivilegeUpdateModel, IRequest<Result>
{
    public GrantPrivilegeCommand(Guid authorityAccountId, string accessPassword,
        Guid subjectAccountId, string privilege)
    {
        base.AuthorityAccountId = authorityAccountId;
        base.AccessPassword = accessPassword;
        base.SubjectAccountId = subjectAccountId;
        base.Privilege = privilege;
    }

    [Required]
    [EnumDataType(typeof(Domain.Enums.UserPrivilege))]
    public new string Privilege
    {
        get { return base.Privilege; }
    }
}
