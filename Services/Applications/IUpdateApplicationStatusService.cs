using RecruitingPlatform.DTOs.Applications;

namespace RecruitingPlatform.Services.Applications;

public interface IUpdateApplicationStatusService
{
    Task<bool> ExecuteAsync(UpdateApplicationStatusDto dto, int employerId);
}