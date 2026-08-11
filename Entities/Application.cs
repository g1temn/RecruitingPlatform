using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingPlatform.Entities;

[Table("applications")]
public class Application
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int VacancyId { get; set; }

    [Required]
    public int ResumeId { get; set; }

    public string CoverLetter { get; set; } = string.Empty;

    [Required]
    public int ApplicationStatusId { get; set; }

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(VacancyId))]
    public Vacancy Vacancy { get; set; } = null!;

    [ForeignKey(nameof(ResumeId))]
    public Resume Resume { get; set; } = null!;

    [ForeignKey(nameof(ApplicationStatusId))]
    public ApplicationStatus ApplicationStatus { get; set; } = null!;
}
