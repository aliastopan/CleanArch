namespace CleanArch.Shared.Interfaces.Models.Identity;

public interface IPrivilegeUpdateModel
{
    Guid AuthorityAccountId { get; }
    Guid SubjectAccountId { get; }
    string AccessPassword { get; }
    string Privilege { get; }
}
