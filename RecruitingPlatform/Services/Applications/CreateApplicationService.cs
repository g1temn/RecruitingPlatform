using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.DTOs.Applications;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Enums;

namespace RecruitingPlatform.Services.Applications;

public class CreateApplicationService(
    RecruitingPlatformDbContext _dbContext)
    : ICreateApplicationService
{
    public async Task<bool> ExecuteAsync(ApplyForVacancyDto dto)
    {
        var searchResult = await _dbContext.Applications.FirstOrDefaultAsync<Application>(v => v.VacancyId == dto.VacancyId && v.ResumeId == dto.ResumeId);
        if (searchResult != null) return false;

        var application = new Application
        {
            VacancyId = dto.VacancyId,
            ResumeId = dto.ResumeId,
            CoverLetter = dto.CoverLetter ?? "",
            ApplicationStatusId = (int)PossibleApplicationStatuses.Applied,
            AppliedAt = DateTime.UtcNow
        };

        var result = await _dbContext.Applications.AddAsync(application);
        if (result == null) return false;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}