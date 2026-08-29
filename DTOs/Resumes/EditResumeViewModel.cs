using RecruitingPlatform.DTOs.Resumes;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.ViewModels.Resumes;

public class EditResumeViewModel
{
    public EditResumeDto FormData { get; set; } = new EditResumeDto();
    public IEnumerable<Specialty> Specialties { get; set; } = new List<Specialty>();
    public Dictionary<string, List<Skill>> GroupedSkills { get; set; } = new();
}