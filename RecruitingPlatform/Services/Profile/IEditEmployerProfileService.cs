using RecruitingPlatform.DTOs.Employer;

namespace RecruitingPlatform.Services.Employers;

public interface IEditEmployerProfileService
{
    Task<EditEmployerDto?> GetProfileForEditAsync(int companyId);
    Task<bool> UpdateProfileAsync(int companyId, EditEmployerDto dto);
}