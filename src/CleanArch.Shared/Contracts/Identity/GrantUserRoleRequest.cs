using CleanArch.Shared.Models.Identity;

namespace CleanArch.Shared.Contracts.Identity;

public class GrantUserRoleRequest : UserRoleUpdateModel
{
    public GrantUserRoleRequest(Guid authorityAccountId, string accessPassword,
        Guid subjectAccountId, string role)
    {
        base.AuthorityAccountId = authorityAccountId;
        base.AccessPassword = accessPassword;
        base.SubjectAccountId = subjectAccountId;
        base.Role = role;
    }
}
