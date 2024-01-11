namespace CleanArch.Shared.Contracts.Identity.Authentication;

public record RefreshAccessResponse(string AccessToken, string RefreshTokenStr);
