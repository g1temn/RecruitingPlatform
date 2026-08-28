using RecruitingPlatform.DTOs.Vacancies;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.ViewModels.Vacancies;

public class CreateVacancyViewModel
{
    public CreateVacancyDto FormData { get; set; } = new CreateVacancyDto();

    public IEnumerable<Specialty> Specialties { get; set; } = new List<Specialty>();
    public IEnumerable<Location> Locations { get; set; } = new List<Location>();
    public IEnumerable<Currency> Currencies { get; set; } = new List<Currency>();

    public Dictionary<string, List<Skill>> GroupedSkills { get; set; } = new();
}