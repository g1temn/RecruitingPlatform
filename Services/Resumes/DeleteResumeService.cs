using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;

namespace RecruitingPlatform.Services.Resumes;

public class DeleteResumeService(RecruitingPlatformDbContext _dbContext) : IDeleteResumeService
{
    public async Task<bool> DeleteAsync(int resumeId, int jobSeekerId)
    {
        var resume = await _dbContext.Resumes
            .FirstOrDefaultAsync(r => r.Id == resumeId && r.JobSeekerId == jobSeekerId && !r.IsDeleted);

        if (resume == null) return false;

        resume.IsDeleted = true;
        await _dbContext.SaveChangesAsync();

        return true;
    }
}