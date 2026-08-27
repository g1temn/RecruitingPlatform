using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.DTOs.Applications;

namespace RecruitingPlatform.Services.Applications;

public class UpdateApplicationStatusService(RecruitingPlatformDbContext _dbContext) : IUpdateApplicationStatusService
{
    public async Task<bool> ExecuteAsync(UpdateApplicationStatusDto dto, int employerId)
    {
        var application = await _dbContext.Applications
            .Include(a => a.Vacancy)
            .FirstOrDefaultAsync(a => a.Id == dto.ApplicationId);

        if (application == null || application.Vacancy?.CompanyId != employerId)
        {
            return false;
        }

        application.ApplicationStatusId = dto.NewStatusId;

        await _dbContext.SaveChangesAsync();
        return true;
    }
}