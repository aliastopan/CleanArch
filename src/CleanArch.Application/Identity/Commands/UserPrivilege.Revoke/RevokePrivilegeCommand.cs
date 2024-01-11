using System.ComponentModel.DataAnnotations;
using CleanArch.Shared.Interfaces.Models.Identity;

namespace CleanArch.Application.Identity.Commands.RevokeUserPrivilege;

public class RevokePrivilegeCommand(Guid authorityAccountId, string accessPassword, Guid subjectAccountId, string privilege)
    : IPrivilegeUpdateModel, IRequest<Result>
{
    [Required]
    public Guid AuthorityAccountId { get; } = authorityAccountId;

    [Required]
    public Guid SubjectAccountId { get; } = subjectAccountId;

    [Required]
    public string AccessPassword { get; } = accessPassword;

    [Required]
    [EnumDataType(typeof(Domain.Enums.UserPrivilege))]
    public string Privilege { get; } = privilege;
}
