using RecruitingPlatform.DTOs.Applications;

namespace RecruitingPlatform.Services.Applications;

public interface ICreateApplicationService
{
    Task<bool> ExecuteAsync(ApplyForVacancyDto dto);
}