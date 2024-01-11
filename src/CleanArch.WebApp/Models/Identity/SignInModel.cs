#nullable disable
using System.ComponentModel.DataAnnotations;
using CleanArch.Shared.Interfaces.Models.Identity;

namespace CleanArch.WebApp.Models.Identity;

public class SignInModel : IAuthenticationModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}
