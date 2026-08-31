using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.DTOs.Vacancies;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Vacancies;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Vacancies;

public class CreateVacancyServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAsync_ValidDto_CreatesAndSavesVacancy()
    {
        using var dbContext = GetInMemoryDbContext();
        var service = new CreateVacancyService(dbContext);

        var dto = new CreateVacancyDto
        {
            Title = "Software Engineer",
            Description = "Great job",
            SpecialtyId = 1,
            LocationId = 1,
            IsRemote = true,
            MinSalary = 1000,
            MaxSalary = 3000,
            SalaryCurrencyId = 1,
            SelectedSkillIds = [10, 20]
        };

        await service.ExecuteAsync(dto, companyId: 100);

        var savedVacancy = await dbContext.Vacancies
            .Include(v => v.VacancySkills)
            .FirstOrDefaultAsync(v => v.Title == "Software Engineer");

        Assert.NotNull(savedVacancy);
        Assert.Equal(100, savedVacancy.CompanyId);
        Assert.True(savedVacancy.IsActive);
        Assert.False(savedVacancy.IsDeleted);
        Assert.Equal(2, savedVacancy.VacancySkills.Count());
    }
}