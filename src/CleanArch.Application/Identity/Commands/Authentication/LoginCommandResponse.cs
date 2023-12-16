namespace CleanArch.Application.Identity.Commands.Authentication;

public record LoginCommandResponse(string AccessToken, string RefreshToken);