using RecruitingPlatform.DTOs.Resumes;

namespace RecruitingPlatform.Services.Resumes;

public interface IEditResumeService
{
    Task<EditResumeDto?> GetForEditAsync(int resumeId, int jobSeekerId, bool isAdmin = false);
    Task<bool> UpdateAsync(int jobSeekerId, EditResumeDto dto, bool isAdmin = false);
}