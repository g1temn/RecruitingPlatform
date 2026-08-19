using RecruitingPlatform.Entities;
using RecruitingPlatform.Enums;
using System.ComponentModel.DataAnnotations;

namespace RecruitingPlatform.DTOs.Auth;

public class SignUpBaseDto
{
    [Required]
    public PossibleUserRole Role { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
