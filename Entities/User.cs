using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingPlatform.Entities;

[Table("users")]
public class User : IdentityUser<int>
{
    [Required]
    [MaxLength(512)]
    public string RefreshToken { get; set; } = string.Empty;

    [Required]
    public DateTime RefreshTokenExpiry { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public bool IsDeleted { get; set; } = false;

    public JobSeeker JobSeeker { get; set; } = null!;
    public Company Company { get; set; } = null!;
}

