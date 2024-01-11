using CleanArch.Shared.Interfaces.Models.Identity;

namespace CleanArch.Shared.Contracts.Identity.UserPrivilege;

public record RevokePrivilegeRequest(Guid AuthorityAccountId, string AccessPassword, Guid SubjectAccountId, string Privilege)
    : IPrivilegeUpdateModel;
