using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Resumes;

public class GetResumeByIdService(
    RecruitingPlatformDbContext _dbContext) 
    : IGetResumeByIdService
{
    public async Task<Resume?> ExecuteAsync(int id)
    {
        return await _dbContext.Resumes
            .AsNoTracking()
            .Include(r => r.JobSeeker)
            .Include(r => r.Specialty)
            .Include(r => r.ResumeSkills)
                .ThenInclude(rs => rs.Skill)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
    }
}
