using System.ComponentModel.DataAnnotations;
using CleanArch.Shared.Contracts.Identity.Authentication;
using CleanArch.Shared.Interfaces.Models.Identity;

namespace CleanArch.Application.Identity.Commands.Authentication;

public class RefreshAccessCommand(string accessToken, string refreshTokenStr)
    : IRefreshAccessModel, IRequest<Result<RefreshAccessResponse>>
{
    [Required]
    public string AccessToken { get; set; } = accessToken;

    [Required]
    public string RefreshTokenStr { get; set; } = refreshTokenStr;
}
