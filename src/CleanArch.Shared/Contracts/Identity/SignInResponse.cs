namespace CleanArch.Shared.Contracts.Identity;

public record SignInResponse(string AccessToken, string RefreshTokenStr);
