using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.DTOs.JobSeekers;

namespace RecruitingPlatform.Services.JobSeekers;

public class EditJobSeekerProfileService(RecruitingPlatformDbContext _dbContext) : IEditJobSeekerProfileService
{
    public async Task<EditJobSeekerDto?> GetProfileForEditAsync(int jobSeekerId)
    {
        var jobSeeker = await _dbContext.JobSeekers
            .FirstOrDefaultAsync(js => js.Id == jobSeekerId && !js.IsDeleted);

        if (jobSeeker == null) return null;

        return new EditJobSeekerDto
        {
            FirstName = jobSeeker.FirstName,
            LastName = jobSeeker.LastName,
            ContactPhone = jobSeeker.ContactPhone,
            ContactEmail = jobSeeker.ContactEmail,
            Birthday = jobSeeker.Birthday
        };
    }

    public async Task<bool> UpdateProfileAsync(int jobSeekerId, EditJobSeekerDto dto)
    {
        var jobSeeker = await _dbContext.JobSeekers
            .FirstOrDefaultAsync(js => js.Id == jobSeekerId && !js.IsDeleted);

        if (jobSeeker == null)
            return false;

        jobSeeker.FirstName = dto.FirstName;
        jobSeeker.LastName = dto.LastName;
        jobSeeker.ContactPhone = dto.ContactPhone;
        jobSeeker.ContactEmail = dto.ContactEmail;
        jobSeeker.Birthday = dto.Birthday;

        await _dbContext.SaveChangesAsync();

        return true;
    }
}