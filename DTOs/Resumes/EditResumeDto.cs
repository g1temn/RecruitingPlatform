using System.ComponentModel.DataAnnotations;
using RecruitingPlatform.Const.Resumes;

namespace RecruitingPlatform.DTOs.Resumes;

public class EditResumeDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = ResumesConstants.SpecialtyRequired)]
    public int SpecialtyId { get; set; }

    [Required(ErrorMessage = ResumesConstants.TitleRequired)]
    [MaxLength(150, ErrorMessage = ResumesConstants.TitleMaxLength)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = ResumesConstants.SummaryRequired)]
    public string Summary { get; set; } = string.Empty;

    public List<int> SelectedSkillIds { get; set; } = new List<int>();
}