using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Resumes;

public interface IGetResumeByIdService
{
    Task<Resume?> ExecuteAsync(int id);
}
