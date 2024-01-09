#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CleanArch.Shared.Models.Identity;

public class RefreshAuthenticationModel
{
    [Required]
    public string AccessToken { get; set; }

    [Required]
    public string RefreshTokenStr { get; set; }
}
