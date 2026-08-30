using System.ComponentModel.DataAnnotations;

namespace RecruitingPlatform.DTOs.Auth;

public class LogInDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
