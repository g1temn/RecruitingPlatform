using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.ViewModels.Profile;

namespace RecruitingPlatform.Services.Profile;

public class GetJobSeekerProfileService(RecruitingPlatformDbContext _dbContext) : IGetJobSeekerProfileService
{
    public async Task<JobSeekerProfileViewModel?> ExecuteAsync(int userId)
    {
        var jobSeeker = await _dbContext.JobSeekers
            .AsNoTracking()
            .Include(js => js.Resumes.Where(r => !r.IsDeleted))
                .ThenInclude(r => r.Specialty)
            .FirstOrDefaultAsync(js => js.Id == userId && !js.IsDeleted);

        if (jobSeeker == null) return null;

        var resumeIds = jobSeeker.Resumes.Select(r => r.Id).ToList();

        var applications = await _dbContext.Applications
            .AsNoTracking()
            .Include(a => a.Vacancy)
                .ThenInclude(v => v.Company)
            .Include(a => a.ApplicationStatus)
            .Where(a => resumeIds.Contains(a.ResumeId))
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();

        return new JobSeekerProfileViewModel
        {
            Id = jobSeeker.Id,
            FirstName = jobSeeker.FirstName,
            LastName = jobSeeker.LastName,
            ContactPhone = jobSeeker.ContactPhone,
            ContactEmail = jobSeeker.ContactEmail,
            Birthday = jobSeeker.Birthday,
            Resumes = jobSeeker.Resumes,
            Applications = applications
        };
    }
}