using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.ViewModels.Profile;

namespace RecruitingPlatform.Services.Profile;

public class GetEmployerProfileService(
    RecruitingPlatformDbContext _dbContext)
    : IGetEmployerProfileService
{
    public async Task<EmployerProfileViewModel?> ExecuteAsync(int userId)
    {
        var  employer = await _dbContext.Companies
            .AsNoTracking()
            .Include(e => e.Vacancies.Where(v => !v.IsDeleted))
                .ThenInclude(v => v.Specialty)
            .Include(e => e.Vacancies.Where(v => !v.IsDeleted))
                .ThenInclude(v => v.Location)
            .FirstOrDefaultAsync(e => e.Id == userId && !e.IsDeleted);

        if (employer == null) return null;

        var vacancyIds = employer.Vacancies.Select(v => v.Id).ToList();

        var recentApplications = await _dbContext.Applications
            .AsNoTracking()
            .Include(a => a.Vacancy)
            .Include(a => a.Resume)
                .ThenInclude(r => r.JobSeeker)
            .Include(a => a.ApplicationStatus)
            .Where(a => vacancyIds.Contains(a.VacancyId))
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();

        return new EmployerProfileViewModel
        {
            Id = employer.Id,
            Name = employer.Name,
            Description = employer.Description ?? "",
            WebsiteUrl = employer.WebsiteUrl,
            ContactPhone = employer.ContactPhone,
            Vacancies = employer.Vacancies,
            RecentApplications = recentApplications
        };
    }
}