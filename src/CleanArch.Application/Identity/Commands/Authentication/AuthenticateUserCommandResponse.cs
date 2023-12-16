namespace CleanArch.Application.Identity.Commands.Authentication;

public record AuthenticateUserCommandResponse(string AccessToken, string RefreshToken);