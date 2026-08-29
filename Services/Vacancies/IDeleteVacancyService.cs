using RecruitingPlatform.DTOs.Vacancies;

namespace RecruitingPlatform.Services.Vacancies;

public interface IDeleteVacancyService
{
    Task<bool> DeleteAsync(int vacancyId, int companyId, bool isAdmin = false);
}