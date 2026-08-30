using RecruitingPlatform.DTOs.Resumes;

namespace RecruitingPlatform.Services.Resumes;

public interface ICreateResumeService
{
    Task<int> ExecuteAsync(CreateResumeDto dto, int jobSeekerId);
}