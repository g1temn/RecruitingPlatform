using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.DTOs.Vacancies;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Vacancies;

public class EditVacancyService(RecruitingPlatformDbContext _dbContext) : IEditVacancyService
{
    public async Task<EditVacancyDto?> GetForEditAsync(int vacancyId, int companyId)
    {
        var vacancy = await _dbContext.Vacancies
            .Include(v => v.VacancySkills)
            .FirstOrDefaultAsync(v => v.Id == vacancyId && v.CompanyId == companyId && !v.IsDeleted);

        if (vacancy == null) return null;

        return new EditVacancyDto
        {
            Id = vacancy.Id,
            Title = vacancy.Title,
            SpecialtyId = vacancy.SpecialtyId,
            Description = vacancy.Description,
            LocationId = vacancy.LocationId,
            IsRemote = vacancy.IsRemote,
            MinSalary = vacancy.MinSalary,
            MaxSalary = vacancy.MaxSalary,
            SalaryCurrencyId = vacancy.SalaryCurrencyId,
            IsActive = vacancy.IsActive,
            SelectedSkillIds = vacancy.VacancySkills.Select(vs => vs.SkillId).ToList()
        };
    }

    public async Task<bool> UpdateAsync(int companyId, EditVacancyDto dto)
    {
        var vacancy = await _dbContext.Vacancies
            .Include(v => v.VacancySkills)
            .FirstOrDefaultAsync(v => v.Id == dto.Id && v.CompanyId == companyId && !v.IsDeleted);

        if (vacancy == null) return false;

        vacancy.Title = dto.Title;
        vacancy.SpecialtyId = dto.SpecialtyId;
        vacancy.Description = dto.Description;
        vacancy.LocationId = dto.LocationId;
        vacancy.IsRemote = dto.IsRemote;
        vacancy.MinSalary = dto.MinSalary;
        vacancy.MaxSalary = dto.MaxSalary;
        vacancy.SalaryCurrencyId = dto.SalaryCurrencyId ?? 1;
        vacancy.IsActive = dto.IsActive;
        vacancy.UpdatedAt = DateTime.UtcNow;

        _dbContext.VacancySkills.RemoveRange(vacancy.VacancySkills);

        if (dto.SelectedSkillIds != null && dto.SelectedSkillIds.Any())
        {
            vacancy.VacancySkills = dto.SelectedSkillIds.Select(skillId => new VacancySkill
            {
                SkillId = skillId
            }).ToList();
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }
}