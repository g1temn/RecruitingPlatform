using RecruitingPlatform.Entities;

namespace RecruitingPlatform.ViewModels.Profile;

public class JobSeekerProfileViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public DateOnly Birthday { get; set; }
    public IEnumerable<Resume> Resumes { get; set; } = new List<Resume>();
    public IEnumerable<Application> Applications { get; set; } = new List<Application>();
}