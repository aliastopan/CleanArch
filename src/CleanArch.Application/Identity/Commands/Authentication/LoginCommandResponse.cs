namespace CleanArch.Application.Identity.Commands.Authentication;

public record LoginCommandResponse(Guid UserAccountId, string AccessToken, string RefreshToken);