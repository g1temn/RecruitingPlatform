using RecruitingPlatform.Entities;
using RecruitingPlatform.Data;
using Microsoft.EntityFrameworkCore;

namespace RecruitingPlatform.Services.Locations;

public class GetAllLocationsService (
    RecruitingPlatformDbContext _dbContext)
    : IGetAllLocationsService
{
    public async Task<IEnumerable<Location>> ExecuteAsync()
    {
        return await _dbContext.Locations.ToListAsync<Location>();
    }
}
