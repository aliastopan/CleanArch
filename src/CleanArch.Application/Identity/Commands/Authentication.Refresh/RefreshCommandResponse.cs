namespace CleanArch.Application.Identity.Commands.Authentication.Refresh;

public record RefreshCommandResponse(string AccessToken, string RefreshToken);
