using CleanArch.Shared.Models.Identity;

namespace CleanArch.Shared.Contracts.Identity;

public class RevokePrivilegeRequest : UserPrivilegeUpdateModel
{
    public RevokePrivilegeRequest(Guid authorityAccountId, string accessPassword,
        Guid subjectAccountId, string privilege)
    {
        base.AuthorityAccountId = authorityAccountId;
        base.AccessPassword = accessPassword;
        base.SubjectAccountId = subjectAccountId;
        base.Privilege = privilege;
    }
}
