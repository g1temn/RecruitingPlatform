using RecruitingPlatform.ViewModels.Profile;

namespace RecruitingPlatform.Services.Profile;

public interface IGetEmployerProfileService
{
    Task<EmployerProfileViewModel?> ExecuteAsync(int userId);
}