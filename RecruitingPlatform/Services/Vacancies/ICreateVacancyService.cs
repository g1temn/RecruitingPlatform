using RecruitingPlatform.DTOs.Vacancies;

namespace RecruitingPlatform.Services.Vacancies;

public interface ICreateVacancyService
{
    Task ExecuteAsync(CreateVacancyDto dto, int companyId);
}