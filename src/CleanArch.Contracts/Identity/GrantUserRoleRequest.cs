namespace CleanArch.Contracts.Identity;

public class GrantUserRoleRequest : UserRoleUpdateModel
{
    public GrantUserRoleRequest(Guid SenderAccountId, string accessPassword,
        Guid subjectAccountId, string role)
    {
        base.SenderAccountId = SenderAccountId;
        base.AccessPassword = accessPassword;
        base.SubjectAccountId = subjectAccountId;
        base.Role = role;
    }
}
