using System.ComponentModel.DataAnnotations;

namespace RecruitingPlatform.DTOs.Auth;

public class SignJobSeekerUpDto : SignUpBaseDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string ContactPhone { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ContactEmail { get; set; } = string.Empty;

    [Required]
    public DateOnly Birthday { get; set; }
}
