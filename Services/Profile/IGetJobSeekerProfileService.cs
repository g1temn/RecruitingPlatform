using RecruitingPlatform.ViewModels.Profile;

namespace RecruitingPlatform.Services.Profile;

public interface IGetJobSeekerProfileService
{
    Task<JobSeekerProfileViewModel?> ExecuteAsync(int userId);
}