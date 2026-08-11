using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingPlatform.Entities;

[Table("job_seekers")]
public class JobSeeker
{
    [Key]
    [ForeignKey(nameof(User))]
    public int Id { get; set; }

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

    [Required]
    public bool IsDeleted { get; set; } = false;

    public User User { get; set; } = null!;

    public IEnumerable<Resume> Resumes { get; set; } = new List<Resume>();
}
