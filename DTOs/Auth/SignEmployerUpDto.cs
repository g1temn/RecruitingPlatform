using System.ComponentModel.DataAnnotations;

namespace RecruitingPlatform.DTOs.Auth;

public class SignEmployerUpDto : SignUpBaseDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? WebsiteUrl { get; set; }

    [Required]
    public string? ContactPhone { get; set; }

    public string? Description { get; set; }
}
