using RecruitingPlatform.Data;
using RecruitingPlatform.DTOs.Vacancies;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Vacancies;

public class CreateVacancyService(RecruitingPlatformDbContext _dbContext) : ICreateVacancyService
{
    public async Task ExecuteAsync(CreateVacancyDto dto, int companyId)
    {
        var vacancy = new Vacancy
        {
            CompanyId = companyId,
            SpecialtyId = dto.SpecialtyId,
            LocationId = dto.LocationId,
            IsRemote = dto.IsRemote,
            Title = dto.Title,
            Description = dto.Description,
            MinSalary = dto.MinSalary,
            MaxSalary = dto.MaxSalary,
            SalaryCurrencyId = dto.SalaryCurrencyId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        if (dto.SelectedSkillIds != null && dto.SelectedSkillIds.Any())
        {
            vacancy.VacancySkills = dto.SelectedSkillIds.Select(skillId => new VacancySkill
            {
                SkillId = skillId
            }).ToList();
        }

        _dbContext.Vacancies.Add(vacancy);
        await _dbContext.SaveChangesAsync();
    }
}