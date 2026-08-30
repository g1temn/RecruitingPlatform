namespace RecruitingPlatform.DTOs.Resumes;

public class ResumeFiltersDto
{
    public int Page { get; set; } = 1;

    public string? SearchQuery { get; set; }

    public int? SpecialtyId { get; set; }
}