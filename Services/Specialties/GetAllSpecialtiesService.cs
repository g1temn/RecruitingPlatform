using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Specialties;

public class GetAllSpecialtiesService(
    RecruitingPlatformDbContext _dbContext) 
    : IGetAllSpecialtiesService
{
    public async Task<IEnumerable<Specialty>> ExecuteAsync()
    {
        return await _dbContext.Specialties
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ToListAsync();
    }
}