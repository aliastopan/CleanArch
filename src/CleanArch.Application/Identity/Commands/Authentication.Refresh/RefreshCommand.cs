namespace CleanArch.Application.Identity.Commands.Authentication.Refresh;

public record RefreshCommand(string AccessToken, string RefreshToken)
    : IRequest<Result<RefreshCommandResponse>>;
