using RecruitingPlatform.DTOs.JobSeekers;

namespace RecruitingPlatform.Services.JobSeekers;

public interface IEditJobSeekerProfileService
{
    Task<EditJobSeekerDto?> GetProfileForEditAsync(int jobSeekerId);
    Task<bool> UpdateProfileAsync(int jobSeekerId, EditJobSeekerDto dto);
}