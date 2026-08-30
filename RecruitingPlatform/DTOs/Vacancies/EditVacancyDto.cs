using System.ComponentModel.DataAnnotations;
using RecruitingPlatform.Const.Vacancies;

namespace RecruitingPlatform.DTOs.Vacancies;

public class EditVacancyDto : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = VacanciesConstants.TitleRequired)]
    [MaxLength(150, ErrorMessage = VacanciesConstants.TitleMaxLength)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = VacanciesConstants.SpecialtyRequired)]
    public int SpecialtyId { get; set; }

    [Required(ErrorMessage = VacanciesConstants.DescriptionRequired)]
    public string Description { get; set; } = string.Empty;

    public int? LocationId { get; set; }
    public bool IsRemote { get; set; }

    [Range(typeof(decimal), "0", "1000000", ErrorMessage = VacanciesConstants.MinSalaryRange)]
    public decimal? MinSalary { get; set; }

    [Range(typeof(decimal), "0", "1000000", ErrorMessage = VacanciesConstants.MaxSalaryRange)]
    public decimal? MaxSalary { get; set; }

    public int? SalaryCurrencyId { get; set; }

    public bool IsActive { get; set; } = true;

    public List<int> SelectedSkillIds { get; set; } = new List<int>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinSalary.HasValue && MaxSalary.HasValue && MinSalary.Value > MaxSalary.Value)
        {
            yield return new ValidationResult(
                VacanciesConstants.InvalidSalaryRange,
                new[] { nameof(MinSalary), nameof(MaxSalary) }
            );
        }

        if ((MinSalary.HasValue || MaxSalary.HasValue) && !SalaryCurrencyId.HasValue)
        {
            yield return new ValidationResult(
                VacanciesConstants.CurrencyRequired,
                new[] { nameof(SalaryCurrencyId) }
            );
        }
    }
}