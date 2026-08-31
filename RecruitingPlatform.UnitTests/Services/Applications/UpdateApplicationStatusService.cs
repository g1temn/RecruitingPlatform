using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.DTOs.Applications;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Applications;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Applications;

public class UpdateApplicationStatusServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAsync_ValidRequest_UpdatesStatusAndReturnsTrue()
    {
        using var dbContext = GetInMemoryDbContext();

        var company = new Company { Id = 1, Name = "Test Company" };
        var vacancy = new Vacancy { Id = 10, CompanyId = 1, Company = company };
        var application = new Application { Id = 100, VacancyId = 10, Vacancy = vacancy, ApplicationStatusId = 1 };

        dbContext.Companies.Add(company);
        dbContext.Vacancies.Add(vacancy);
        dbContext.Applications.Add(application);
        await dbContext.SaveChangesAsync();

        var service = new UpdateApplicationStatusService(dbContext);
        var dto = new UpdateApplicationStatusDto { ApplicationId = 100, NewStatusId = 2 };

        var result = await service.ExecuteAsync(dto, 1);

        Assert.True(result);

        var updatedApplication = await dbContext.Applications.FirstOrDefaultAsync(a => a.Id == 100);
        Assert.NotNull(updatedApplication);
        Assert.Equal(2, updatedApplication.ApplicationStatusId);
    }

    [Fact]
    public async Task ExecuteAsync_ApplicationBelongsToDifferentEmployer_ReturnsFalse()
    {
        using var dbContext = GetInMemoryDbContext();

        var company = new Company { Id = 1, Name = "Test Company" };
        var vacancy = new Vacancy { Id = 10, CompanyId = 1, Company = company };
        var application = new Application { Id = 100, VacancyId = 10, Vacancy = vacancy, ApplicationStatusId = 1 };

        dbContext.Companies.Add(company);
        dbContext.Vacancies.Add(vacancy);
        dbContext.Applications.Add(application);
        await dbContext.SaveChangesAsync();

        var service = new UpdateApplicationStatusService(dbContext);
        var dto = new UpdateApplicationStatusDto { ApplicationId = 100, NewStatusId = 2 };

        var result = await service.ExecuteAsync(dto, 99);

        Assert.False(result);

        var unchangedApplication = await dbContext.Applications.FirstOrDefaultAsync(a => a.Id == 100);
        Assert.NotNull(unchangedApplication);
        Assert.Equal(1, unchangedApplication.ApplicationStatusId);
    }

    [Fact]
    public async Task ExecuteAsync_ApplicationNotFound_ReturnsFalse()
    {
        using var dbContext = GetInMemoryDbContext();
        var service = new UpdateApplicationStatusService(dbContext);
        var dto = new UpdateApplicationStatusDto { ApplicationId = 999, NewStatusId = 2 };

        var result = await service.ExecuteAsync(dto, 1);

        Assert.False(result);
    }
}