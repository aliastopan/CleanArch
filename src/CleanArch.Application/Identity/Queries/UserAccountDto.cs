namespace CleanArch.Application.Identity.Queries;

public record UserAccountDto(Guid Username, string UserRole, DateTime LastLoggedIn);

