namespace CleanArch.Shared.Interfaces.Models.Identity;

public interface IRoleUpdateModel
{
    Guid AuthorityAccountId { get; }
    Guid SubjectAccountId { get; }
    string AccessPassword { get; }
    string Role { get; }
}