using RecruitingPlatform.DTOs.Applications;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.ViewModels.Applications;

public class ApplyViewModel
{
    public Vacancy Vacancy { get; set; } = null!;
    public IEnumerable<Resume> UserResumes { get; set; } = new List<Resume>();
    public ApplyForVacancyDto FormData { get; set; } = new ApplyForVacancyDto();
}