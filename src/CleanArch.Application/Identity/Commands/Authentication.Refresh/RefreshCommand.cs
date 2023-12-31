namespace CleanArch.Application.Identity.Commands.Authentication.Refresh;

public record RefreshCommand(string AccessToken, string RefreshTokenStr)
    : IRequest<Result<RefreshCommandResponse>>;
