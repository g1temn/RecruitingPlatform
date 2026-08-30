using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Const.Vacancies;
using RecruitingPlatform.DTOs.Vacancies;
using RecruitingPlatform.DTOs.Common;

namespace RecruitingPlatform.Services.Vacancies;

public class GetVacanciesWithFiltersService(
    RecruitingPlatformDbContext _dbContext) 
    : IGetVacanciesWithFiltersService
{
    public async Task<PagedResultDto<Vacancy>> ExecuteAsync(VacancyFiltersDto dto)
    {
        var query = _dbContext.Vacancies
            .AsNoTracking()
            .Include(v => v.Company)
            .Include(v => v.Location)
            .Include(v => v.Specialty)
            .Include(v => v.Currency)
            .Include(v => v.VacancySkills)
                .ThenInclude(vs => vs.Skill)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(dto.SearchQuery))
        {
            var search = dto.SearchQuery.ToLower();
            query = query.Where(v =>
                v.Title.ToLower().Contains(search) ||
                v.Description.ToLower().Contains(search) ||
                (v.Company != null && v.Company.Name.ToLower().Contains(search)) ||
                (v.Specialty != null && v.Specialty.Name.ToLower().Contains(search)) ||
                v.VacancySkills.Any(vs => vs.Skill != null && vs.Skill.Name.ToLower().Contains(search))
            );
        }

        if (dto.SpecialtyId.HasValue)
            query = query.Where(v => v.SpecialtyId == dto.SpecialtyId);

        if (dto.LocationId.HasValue)
            query = query.Where(v => v.LocationId == dto.LocationId);

        if (dto.IsRemote.HasValue)
            query = query.Where(v => v.IsRemote == dto.IsRemote.Value);

        if (dto.MinSalary.HasValue)
            query = query.Where(v => v.MaxSalary >= dto.MinSalary.Value || v.MinSalary >= dto.MinSalary.Value);

        if (dto.MaxSalary.HasValue)
            query = query.Where(v => v.MinSalary <= dto.MaxSalary.Value || v.MaxSalary <= dto.MaxSalary.Value);

        if (dto.SkillIds != null && dto.SkillIds.Any())
            query = query.Where(v => v.VacancySkills.Any(vs => dto.SkillIds.Contains(vs.SkillId)));

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)VacanciesConstants.NumberOfVacanciesOnOnePage);

        var pagedVacancies = await query
            .OrderByDescending(v => v.CreatedAt)
            .Skip((dto.Page - 1) * VacanciesConstants.NumberOfVacanciesOnOnePage)
            .Take(VacanciesConstants.NumberOfVacanciesOnOnePage)
            .ToListAsync();

        return new PagedResultDto<Vacancy>
        {
            Items = pagedVacancies,
            TotalItems = totalItems,
            TotalPages = totalPages,
            CurrentPage = dto.Page
        };
    }
}