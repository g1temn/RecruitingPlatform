using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Skills;

public class GetAllSkillsService(RecruitingPlatformDbContext _dbContext) : IGetAllSkillsService
{
    public async Task<IEnumerable<Skill>> ExecuteAsync()
    {
        return await _dbContext.Skills
            .Include(s => s.SkillType)
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ToListAsync();
    }
}