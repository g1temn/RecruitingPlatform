using System.ComponentModel.DataAnnotations;

namespace RecruitingPlatform.DTOs.Applications;

public class ApplyForVacancyDto
{
    [Required]
    public int VacancyId { get; set; }

    [Required]
    public int ResumeId { get; set; }

    public string? CoverLetter { get; set; }
}