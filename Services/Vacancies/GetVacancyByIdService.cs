using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Vacancies;

public class GetVacancyByIdService (
    RecruitingPlatformDbContext _dbContext) 
    : IGetVacancyByIdService
{
    public async Task<Vacancy?> ExecuteAsync(int id)
    {
        var query = _dbContext.Vacancies
            .AsNoTracking()
            .Include(v => v.Company)
            .Include(v => v.Location)
            .Include(v => v.Specialty) 
            .Include(v => v.Currency)
            .Include(v => v.VacancySkills)
                .ThenInclude(vs => vs.Skill)
            .AsQueryable();

        var vacancyWithRelatedData = await query.FirstOrDefaultAsync<Vacancy>(v => v.Id == id);

        return vacancyWithRelatedData;
    }
}
