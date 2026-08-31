using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.DTOs.Vacancies;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Vacancies;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Vacancies;

public class EditVacancyServiceTests : DbTestBase
{
    [Fact]
    public async Task GetForEditAsync_ValidIdAndCompany_ReturnsDto()
    {
        using var dbContext = GetInMemoryDbContext();
        dbContext.Vacancies.Add(new Vacancy
        {
            Id = 1,
            CompanyId = 50,
            Title = "Backend Dev",
            Description = "C#",
            SpecialtyId = 1,
            LocationId = 1,
            VacancySkills = [new VacancySkill { SkillId = 5 }]
        });
        await dbContext.SaveChangesAsync();

        var service = new EditVacancyService(dbContext);
        var result = await service.GetForEditAsync(1, companyId: 50);

        Assert.NotNull(result);
        Assert.Equal("Backend Dev", result.Title);
        Assert.Single(result.SelectedSkillIds);
        Assert.Equal(5, result.SelectedSkillIds.First());
    }

    [Fact]
    public async Task UpdateAsync_ValidChanges_UpdatesAndReturnsTrue()
    {
        using var dbContext = GetInMemoryDbContext();
        dbContext.Vacancies.Add(new Vacancy
        {
            Id = 1,
            CompanyId = 50,
            Title = "Old Title",
            Description = "Old Desc",
            SpecialtyId = 1,
            LocationId = 1
        });
        await dbContext.SaveChangesAsync();

        var service = new EditVacancyService(dbContext);
        var dto = new EditVacancyDto
        {
            Id = 1,
            Title = "New Title",
            Description = "New Desc",
            SpecialtyId = 2,
            LocationId = 2,
            IsRemote = true,
            MinSalary = 2000,
            MaxSalary = 4000,
            IsActive = true,
            SelectedSkillIds = [10]
        };

        var result = await service.UpdateAsync(companyId: 50, dto: dto);

        Assert.True(result);
        var updated = await dbContext.Vacancies.Include(v => v.VacancySkills).FirstAsync(v => v.Id == 1);
        Assert.Equal("New Title", updated.Title);
        Assert.Equal(2, updated.SpecialtyId);
        Assert.Single(updated.VacancySkills);
        Assert.Equal(10, updated.VacancySkills.First().SkillId);
    }
}