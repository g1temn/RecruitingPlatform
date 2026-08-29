namespace RecruitingPlatform.Services.Resumes;

public interface IDeleteResumeService
{
    Task<bool> DeleteAsync(int resumeId, int jobSeekerId);
}