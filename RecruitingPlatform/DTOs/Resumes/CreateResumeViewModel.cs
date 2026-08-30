using RecruitingPlatform.DTOs.Resumes;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.ViewModels.Resumes;

public class CreateResumeViewModel
{
    public CreateResumeDto FormData { get; set; } = new CreateResumeDto();

    public IEnumerable<Specialty> Specialties { get; set; } = new List<Specialty>();

    public Dictionary<string, List<Skill>> GroupedSkills { get; set; } = new();
}