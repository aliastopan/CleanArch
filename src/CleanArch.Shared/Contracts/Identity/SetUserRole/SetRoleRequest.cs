using CleanArch.Shared.Interfaces.Models.Identity;

namespace CleanArch.Shared.Contracts.Identity.SetUserRole;

public record SetRoleRequest(Guid AuthorityAccountId, string AccessPassword, Guid SubjectAccountId, string Role)
    : IRoleUpdateModel;