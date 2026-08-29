using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;

namespace RecruitingPlatform.Services.Vacancies;

public class DeleteVacancyService(RecruitingPlatformDbContext _dbContext) : IDeleteVacancyService
{
    public async Task<bool> DeleteAsync(int vacancyId, int companyId, bool isAdmin = false)
    {
        var vacancy = await _dbContext.Vacancies
            .FirstOrDefaultAsync(v => v.Id == vacancyId && (v.CompanyId == companyId || isAdmin) && !v.IsDeleted);

        if (vacancy == null) return false;

        vacancy.IsDeleted = true;
        await _dbContext.SaveChangesAsync();

        return true;
    }
}