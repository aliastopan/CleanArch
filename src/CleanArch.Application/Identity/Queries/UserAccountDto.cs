namespace CleanArch.Application.Identity.Queries;

public record UserAccountDto(Guid Username, List<string> UserRoles, DateTime LastLoggedIn);

