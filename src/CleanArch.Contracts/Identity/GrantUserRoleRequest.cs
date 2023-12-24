namespace CleanArch.Contracts.Identity;

public class GrantUserRoleRequest : UserRoleUpdateModel
{
    public GrantUserRoleRequest(Guid AuthorityAccountId, string accessPassword,
        Guid subjectAccountId, string role)
    {
        base.AuthorityAccountId = AuthorityAccountId;
        base.AccessPassword = accessPassword;
        base.SubjectAccountId = subjectAccountId;
        base.Role = role;
    }
}
