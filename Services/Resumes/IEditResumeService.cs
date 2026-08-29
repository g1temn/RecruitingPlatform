using RecruitingPlatform.DTOs.Resumes;

namespace RecruitingPlatform.Services.Resumes;

public interface IEditResumeService
{
    Task<EditResumeDto?> GetForEditAsync(int resumeId, int jobSeekerId);
    Task<bool> UpdateAsync(int jobSeekerId, EditResumeDto dto);
}