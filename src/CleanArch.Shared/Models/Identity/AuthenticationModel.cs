#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CleanArch.Shared.Models.Identity;

public class AuthenticationModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}
