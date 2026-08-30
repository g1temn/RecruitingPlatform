using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.DTOs.Common;
using RecruitingPlatform.DTOs.Resumes;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Const.Resumes;

namespace RecruitingPlatform.Services.Resumes;

public class GetResumesWithFiltersService(
    RecruitingPlatformDbContext _dbContext) 
    : IGetResumesWithFiltersService
{
    public async Task<PagedResultDto<Resume>> ExecuteAsync(ResumeFiltersDto filters)
    {
        var query = _dbContext.Resumes
            .AsNoTracking()
            .Include(r => r.JobSeeker)
            .Include(r => r.Specialty)
            .Include(r => r.ResumeSkills)
                .ThenInclude(rs => rs.Skill)
            .AsQueryable();

        if (filters.SpecialtyId.HasValue)
        {
            query = query.Where(r => r.SpecialtyId == filters.SpecialtyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.SearchQuery))
        {
            var search = filters.SearchQuery.ToLower();
            query = query.Where(r =>
                r.Title.ToLower().Contains(search) ||
                r.Summary.ToLower().Contains(search) ||
                (r.JobSeeker.FirstName + " " + r.JobSeeker.LastName).ToLower().Contains(search) ||
                (r.Specialty != null && r.Specialty.Name.ToLower().Contains(search)) ||
                r.ResumeSkills.Any(rs => rs.Skill != null && rs.Skill.Name.ToLower().Contains(search))
            );
        }

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)ResumesConstants.NumberOfResumesOnOnePage);

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((filters.Page - 1) * ResumesConstants.NumberOfResumesOnOnePage)
            .Take(ResumesConstants.NumberOfResumesOnOnePage)
            .ToListAsync();

        return new PagedResultDto<Resume>
        {
            Items = items,
            CurrentPage = filters.Page,
            TotalPages = totalPages,
            TotalItems = totalItems
        };
    }
}