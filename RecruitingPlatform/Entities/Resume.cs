using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingPlatform.Entities;

[Table("resumes")]
public class Resume
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int JobSeekerId { get; set; }

    [Required]
    public int SpecialtyId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Summary { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public bool IsDeleted { get; set; } = false;

    [ForeignKey(nameof(JobSeekerId))]
    public JobSeeker JobSeeker { get; set; } = null!;

    [ForeignKey(nameof(SpecialtyId))]
    public Specialty Specialty { get; set; } = null!;

    public IEnumerable<ResumeSkill> ResumeSkills { get; set; } = new List<ResumeSkill>();
    public IEnumerable<Application> Applications { get; set; } = new List<Application>();
}
