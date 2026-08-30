using RecruitingPlatform.DTOs.Common;
using RecruitingPlatform.DTOs.Vacancies;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Vacancies;

public interface IGetVacanciesWithFiltersService
{
    Task<PagedResultDto<Vacancy>> ExecuteAsync(VacancyFiltersDto dto);
}