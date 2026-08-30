using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Applications;

public class GetApplicationForReviewService(
    RecruitingPlatformDbContext _dbContext) 
    : IGetApplicationForReviewService
{
    public async Task<Application?> ExecuteAsync(int applicationId, int employerId)
    {
        return await _dbContext.Applications
            .AsNoTracking()
            .Include(a => a.Vacancy)
            .Include(a => a.ApplicationStatus)
            .Include(a => a.Resume)
                .ThenInclude(r => r.JobSeeker)
            .Include(a => a.Resume)
                .ThenInclude(r => r.Specialty)
             .Include(a => a.Resume)
                 .ThenInclude(r => r.ResumeSkills)
                     .ThenInclude(rs => rs.Skill)
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.Vacancy.CompanyId == employerId);
    }
}