using RecruitingPlatform.DTOs.Vacancies;

namespace RecruitingPlatform.Services.Vacancies;

public interface IEditVacancyService
{
    Task<EditVacancyDto?> GetForEditAsync(int vacancyId, int companyId);
    Task<bool> UpdateAsync(int companyId, EditVacancyDto dto);
}