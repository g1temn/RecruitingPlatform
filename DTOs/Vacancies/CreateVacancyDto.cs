using System.ComponentModel.DataAnnotations;
using RecruitingPlatform.Const.Vacancies;

namespace RecruitingPlatform.DTOs.Vacancies;

public class CreateVacancyDto
{
    [Required(ErrorMessage = VacanciesConstants.TitleRequired)]
    [MaxLength(150, ErrorMessage = VacanciesConstants.TitleMaxLength)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = VacanciesConstants.SpecialtyRequired)]
    public int SpecialtyId { get; set; }

    public int? LocationId { get; set; }

    public bool IsRemote { get; set; }

    [Required(ErrorMessage = VacanciesConstants.DescriptionRequired)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 1000000, ErrorMessage = VacanciesConstants.MinSalaryRange)]
    public decimal? MinSalary { get; set; }

    [Range(0, 1000000, ErrorMessage = VacanciesConstants.MaxSalaryRange)]
    public decimal? MaxSalary { get; set; }

    [Required(ErrorMessage = VacanciesConstants.CurrencyRequired)]
    public int SalaryCurrencyId { get; set; }

    public List<int> SelectedSkillIds { get; set; } = new List<int>();
}