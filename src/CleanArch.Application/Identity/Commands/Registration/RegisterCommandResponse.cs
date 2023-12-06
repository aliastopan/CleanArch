namespace CleanArch.Application.Identity.Commands.Registration;

public record RegisterCommandResponse(Guid Id,
    string Username,
    string Email,
    string Role);
