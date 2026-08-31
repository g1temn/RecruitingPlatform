using RecruitingPlatform.DTOs.Vacancies;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Vacancies;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Vacancies;

public class GetVacanciesWithFiltersServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAsync_FilterBySpecialty_ReturnsMatchingVacancies()
    {
        using var dbContext = GetInMemoryDbContext();

        var company = new Company { Id = 100, Name = "Tech Corp" };
        var specialty1 = new Specialty { Id = 10, Name = "Developer" };
        var specialty2 = new Specialty { Id = 20, Name = "QA" };
        var location = new Location { Id = 1, City = "TestCity" };
        var currency = new Currency { Id = 1, Name = "USD" };

        dbContext.Companies.Add(company);
        dbContext.Specialties.AddRange(specialty1, specialty2);
        dbContext.Locations.Add(location);
        dbContext.Currencies.Add(currency);

        dbContext.Vacancies.AddRange(
            new Vacancy { Id = 1, Title = "A", Description = "Test", SpecialtyId = 10, Specialty = specialty1, CompanyId = 100, Company = company, LocationId = 1, Location = location, SalaryCurrencyId = 1, Currency = currency, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Vacancy { Id = 2, Title = "B", Description = "Test", SpecialtyId = 20, Specialty = specialty2, CompanyId = 100, Company = company, LocationId = 1, Location = location, SalaryCurrencyId = 1, Currency = currency, IsDeleted = false, CreatedAt = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var service = new GetVacanciesWithFiltersService(dbContext);
        var filters = new VacancyFiltersDto { Page = 1, SpecialtyId = 10 };

        var result = await service.ExecuteAsync(filters);

        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(1, result.Items.First().Id);
    }

    [Fact]
    public async Task ExecuteAsync_FilterBySearchQuery_ReturnsMatchingVacancies()
    {
        using var dbContext = GetInMemoryDbContext();

        var company = new Company { Id = 200, Name = "Tech Corp" };
        var specialty = new Specialty { Id = 11, Name = "Developer" };
        var location = new Location { Id = 1, City = "TestCity" };
        var currency = new Currency { Id = 1, Name = "USD" };

        dbContext.Companies.Add(company);
        dbContext.Specialties.Add(specialty);
        dbContext.Locations.Add(location);
        dbContext.Currencies.Add(currency);

        dbContext.Vacancies.AddRange(
            new Vacancy { Id = 3, Title = "UniqueTitleSearch", Description = "text", SpecialtyId = 11, Specialty = specialty, CompanyId = 200, Company = company, LocationId = 1, Location = location, SalaryCurrencyId = 1, Currency = currency, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Vacancy { Id = 4, Title = "Other", Description = "text", SpecialtyId = 11, Specialty = specialty, CompanyId = 200, Company = company, LocationId = 1, Location = location, SalaryCurrencyId = 1, Currency = currency, IsDeleted = false, CreatedAt = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var service = new GetVacanciesWithFiltersService(dbContext);
        var filters = new VacancyFiltersDto { Page = 1, SearchQuery = "uniquetitle" };

        var result = await service.ExecuteAsync(filters);

        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(3, result.Items.First().Id);
    }

    [Fact]
    public async Task ExecuteAsync_NoFilters_ReturnsAllPaged()
    {
        using var dbContext = GetInMemoryDbContext();

        var company = new Company { Id = 300, Name = "Tech Corp" };
        var specialty = new Specialty { Id = 12, Name = "QA" };
        var location = new Location { Id = 1, City = "TestCity" };
        var currency = new Currency { Id = 1, Name = "USD" };

        dbContext.Companies.Add(company);
        dbContext.Specialties.Add(specialty);
        dbContext.Locations.Add(location);
        dbContext.Currencies.Add(currency);

        dbContext.Vacancies.AddRange(
            new Vacancy { Id = 5, Title = "A", Description = "Test", SpecialtyId = 12, Specialty = specialty, CompanyId = 300, Company = company, LocationId = 1, Location = location, SalaryCurrencyId = 1, Currency = currency, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Vacancy { Id = 6, Title = "B", Description = "Test", SpecialtyId = 12, Specialty = specialty, CompanyId = 300, Company = company, LocationId = 1, Location = location, SalaryCurrencyId = 1, Currency = currency, IsDeleted = false, CreatedAt = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var service = new GetVacanciesWithFiltersService(dbContext);
        var filters = new VacancyFiltersDto { Page = 1 };

        var result = await service.ExecuteAsync(filters);

        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalItems);
    }
}