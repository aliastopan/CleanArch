using System.ComponentModel.DataAnnotations;
using CleanArch.Shared.Contracts.Identity.Authentication;
using CleanArch.Shared.Interfaces.Models.Identity;

namespace CleanArch.Application.Identity.Commands.Authentication;

public class SignInCommand(string username, string password)
    : IAuthenticationModel, IRequest<Result<SignInResponse>>
{
    [Required]
    public string Username { get; } = username;

    [Required]
    public string Password { get; } = password;
}
