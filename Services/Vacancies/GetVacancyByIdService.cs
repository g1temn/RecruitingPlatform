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
        var vacancy = await _dbContext.Vacancies.FirstOrDefaultAsync<Vacancy>(v => v.Id == id);
        return vacancy;
    }
}
