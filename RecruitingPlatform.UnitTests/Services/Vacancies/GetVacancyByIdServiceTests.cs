using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Vacancies;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Vacancies;

public class GetVacancyByIdServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAsync_ExistingId_ReturnsVacancy()
    {
        using var dbContext = GetInMemoryDbContext();

        var company = new Company { Id = 1, Name = "Tech Corp" };
        var specialty = new Specialty { Id = 10, Name = "Backend" };
        var location = new Location { Id = 5, City = "Kyiv" };
        var currency = new Currency { Id = 2, Name = "USD" };
        var skill = new Skill { Id = 1, Name = "C#" };

        dbContext.Companies.Add(company);
        dbContext.Specialties.Add(specialty);
        dbContext.Locations.Add(location);
        dbContext.Currencies.Add(currency);
        dbContext.Skills.Add(skill);

        var vacancy = new Vacancy
        {
            Id = 10,
            Title = "Senior Dev",
            Description = "Lead the team",
            CompanyId = 1,
            Company = company,
            SpecialtyId = 10,
            Specialty = specialty,
            LocationId = 5,
            Location = location,
            SalaryCurrencyId = 2,
            Currency = currency,
            IsDeleted = false,
            VacancySkills = new List<VacancySkill>
            {
                new VacancySkill { SkillId = 1, Skill = skill }
            }
        };

        dbContext.Vacancies.Add(vacancy);
        await dbContext.SaveChangesAsync();

        var service = new GetVacancyByIdService(dbContext);

        var result = await service.ExecuteAsync(10);

        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("Senior Dev", result.Title);
        Assert.NotNull(result.Company);
        Assert.NotNull(result.Specialty);
        Assert.NotNull(result.Location);
        Assert.NotNull(result.Currency);
        Assert.Single(result.VacancySkills);
        Assert.NotNull(result.VacancySkills.First().Skill);
    }

    [Fact]
    public async Task ExecuteAsync_NonExistingId_ReturnsNull()
    {
        using var dbContext = GetInMemoryDbContext();
        var service = new GetVacancyByIdService(dbContext);

        var result = await service.ExecuteAsync(999);

        Assert.Null(result);
    }
}