namespace CleanArch.Application.Identity.Commands.Authentication;

public record SignInCommandResponse(string AccessToken, string RefreshToken);