using System.ComponentModel.DataAnnotations;

namespace RecruitingPlatform.DTOs.Resumes;

public class CreateResumeDto
{
    [Required(ErrorMessage = "Будь ласка, оберіть спеціальність.")]
    public int SpecialtyId { get; set; }

    [Required(ErrorMessage = "Посада (заголовок) є обов'язковою.")]
    [MaxLength(150, ErrorMessage = "Заголовок не може перевищувати 150 символів.")]
    public string Title { get; set; } = string.Empty;

    public string? Summary { get; set; }

    public List<int> SelectedSkillIds { get; set; } = new List<int>();
}