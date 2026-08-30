using RecruitingPlatform.DTOs.Vacancies;

namespace RecruitingPlatform.Services.Vacancies;

public interface IEditVacancyService
{
    Task<EditVacancyDto?> GetForEditAsync(int vacancyId, int companyId, bool isAdmin = false);
    Task<bool> UpdateAsync(int companyId, EditVacancyDto dto, bool isAdmin = false);
}