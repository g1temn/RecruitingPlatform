using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Vacancies;

public interface IGetVacancyByIdService
{
    Task<Vacancy?> ExecuteAsync(int id);
}
