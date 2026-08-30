using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Applications;

public class GetAllApplicationStatusesService(
    RecruitingPlatformDbContext _dbContext) 
    : IGetAllApplicationStatusesService
{
    public async Task<IEnumerable<ApplicationStatus>> ExecuteAsync()
    {
        return await _dbContext.ApplicationStatuses
            .AsNoTracking()
            .ToListAsync();
    }
}