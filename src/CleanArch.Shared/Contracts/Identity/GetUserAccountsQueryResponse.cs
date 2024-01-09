#nullable disable

namespace CleanArch.Shared.Contracts.Identity;

public record GetUserAccountsQueryResponse(List<UserAccountDto> UserAccountDtos);

public record UserAccountDto
{
    public Guid UserAccountId { get; set; }
    public List<string> UserPrivileges { get; set; }
    public DateTime LastLoggedIn { get; set; }
}