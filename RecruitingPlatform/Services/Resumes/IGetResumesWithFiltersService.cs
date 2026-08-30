using RecruitingPlatform.DTOs.Common;
using RecruitingPlatform.Entities;
using RecruitingPlatform.DTOs.Resumes;

namespace RecruitingPlatform.Services.Resumes;

public interface IGetResumesWithFiltersService
{
    Task<PagedResultDto<Resume>> ExecuteAsync(ResumeFiltersDto filters);
}