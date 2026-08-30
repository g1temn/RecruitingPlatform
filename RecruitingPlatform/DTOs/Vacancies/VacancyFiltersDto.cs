namespace RecruitingPlatform.DTOs.Vacancies;

public class VacancyFiltersDto
{
    public string? SearchQuery { get; set; }
    public int Page { get; set; } = 1;
    public int? SpecialtyId { get; set; }
    public int? IndustryId { get; set; }
    public IEnumerable<int>? SkillIds { get; set; }
    public int? LocationId { get; set; }
    public bool? IsRemote { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
}
