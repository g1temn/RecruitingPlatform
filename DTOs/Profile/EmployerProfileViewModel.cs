using RecruitingPlatform.Entities;

namespace RecruitingPlatform.ViewModels.Profile;

public class EmployerProfileViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? WebsiteUrl { get; set; }
    public string? ContactPhone { get; set; }
    public IEnumerable<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
    public IEnumerable<Application> RecentApplications { get; set; } = new List<Application>();
}