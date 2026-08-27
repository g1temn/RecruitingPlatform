using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Applications;

public interface IGetAllApplicationStatusesService
{
    Task<IEnumerable<ApplicationStatus>> ExecuteAsync();
}