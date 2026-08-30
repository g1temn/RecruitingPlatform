using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Resumes;

public class GetActiveResumesByJobSeekerIdService(
    RecruitingPlatformDbContext _dbContext) 
    : IGetActiveResumesByJobSeekerIdService
{
    public async Task<IEnumerable<Resume>> ExecuteAsync(int jobSeekerId)
    {
        return await _dbContext.Resumes
            .AsNoTracking()
            .Where(r => r.JobSeekerId == jobSeekerId && !r.IsDeleted)
            .ToListAsync();
    }
}