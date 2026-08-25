using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Resumes;

public interface IGetActiveResumesByJobSeekerIdService
{
    Task<IEnumerable<Resume>> ExecuteAsync(int jobSeekerId);
}