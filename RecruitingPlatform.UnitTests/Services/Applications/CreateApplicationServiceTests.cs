using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.DTOs.Applications;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Enums;
using RecruitingPlatform.Services.Applications;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Applications;

public class CreateApplicationServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAsync_ApplicationAlreadyExists_ReturnsFalse()
    {
        using var dbContext = GetInMemoryDbContext();

        var existingApplication = new Application
        {
            VacancyId = 1,
            ResumeId = 2,
            CoverLetter = "Існуючий супровідний лист",
            ApplicationStatusId = (int)PossibleApplicationStatuses.Applied,
            AppliedAt = DateTime.UtcNow
        };
        dbContext.Applications.Add(existingApplication);
        await dbContext.SaveChangesAsync();

        var service = new CreateApplicationService(dbContext);
        var dto = new ApplyForVacancyDto { VacancyId = 1, ResumeId = 2 };

        var result = await service.ExecuteAsync(dto);

        Assert.False(result);
        Assert.Equal(1, await dbContext.Applications.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_NewApplication_AddsToDbAndReturnsTrue()
    {
        using var dbContext = GetInMemoryDbContext();
        var service = new CreateApplicationService(dbContext);

        var dto = new ApplyForVacancyDto
        {
            VacancyId = 10,
            ResumeId = 20,
            CoverLetter = "Мій супровідний лист"
        };

        var result = await service.ExecuteAsync(dto);

        Assert.True(result);
        var savedApplication = await dbContext.Applications.FirstOrDefaultAsync();
        Assert.NotNull(savedApplication);
        Assert.Equal(10, savedApplication.VacancyId);
        Assert.Equal(20, savedApplication.ResumeId);
    }

    [Fact]
    public async Task ExecuteAsync_NullCoverLetter_SavesWithEmptyString()
    {
        using var dbContext = GetInMemoryDbContext();
        var service = new CreateApplicationService(dbContext);
        var dto = new ApplyForVacancyDto { VacancyId = 5, ResumeId = 15, CoverLetter = null };

        var result = await service.ExecuteAsync(dto);

        Assert.True(result);
        var savedApplication = await dbContext.Applications.FirstOrDefaultAsync();
        Assert.Equal(string.Empty, savedApplication.CoverLetter);
    }
}