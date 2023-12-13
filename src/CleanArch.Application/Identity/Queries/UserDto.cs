namespace CleanArch.Application.Identity.Queries;

public record UserDto(Guid Username, string Role, DateTime LastLoggedIn);

