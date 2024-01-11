namespace CleanArch.Shared.Contracts.Identity.Authentication;

public record SignInResponse(string AccessToken, string RefreshTokenStr);
