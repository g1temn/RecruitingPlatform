using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Vacancies;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Vacancies;

public class DeleteVacancyServiceTests : DbTestBase
{
    [Fact]
    public async Task DeleteAsync_VacancyBelongsToCompany_SoftDeletesAndReturnsTrue()
    {
        using var dbContext = GetInMemoryDbContext();
        var vacancyId = 1;
        dbContext.Vacancies.Add(new Vacancy { Id = vacancyId, CompanyId = 100, Title = "Dev", Description = "Desc", IsDeleted = false });
        await dbContext.SaveChangesAsync();

        var service = new DeleteVacancyService(dbContext);
        var result = await service.DeleteAsync(vacancyId, companyId: 100);

        Assert.True(result);
        var deletedVacancy = await dbContext.Vacancies.IgnoreQueryFilters().FirstOrDefaultAsync(v => v.Id == vacancyId);
        Assert.NotNull(deletedVacancy);
        Assert.True(deletedVacancy.IsDeleted);
    }

    [Fact]
    public async Task DeleteAsync_UserIsAdmin_SoftDeletesAndReturnsTrue()
    {
        using var dbContext = GetInMemoryDbContext();
        var vacancyId = 2;
        dbContext.Vacancies.Add(new Vacancy { Id = vacancyId, CompanyId = 100, Title = "Dev", Description = "Desc", IsDeleted = false });
        await dbContext.SaveChangesAsync();

        var service = new DeleteVacancyService(dbContext);
        var result = await service.DeleteAsync(vacancyId, companyId: 999, isAdmin: true);

        Assert.True(result);
        var deletedVacancy = await dbContext.Vacancies.IgnoreQueryFilters().FirstOrDefaultAsync(v => v.Id == vacancyId);
        Assert.NotNull(deletedVacancy);
        Assert.True(deletedVacancy.IsDeleted);
    }

    [Fact]
    public async Task DeleteAsync_VacancyBelongsToDifferentCompany_ReturnsFalse()
    {
        using var dbContext = GetInMemoryDbContext();
        var vacancyId = 3;
        dbContext.Vacancies.Add(new Vacancy { Id = vacancyId, CompanyId = 100, Title = "Dev", Description = "Desc", IsDeleted = false });
        await dbContext.SaveChangesAsync();

        var service = new DeleteVacancyService(dbContext);
        var result = await service.DeleteAsync(vacancyId, companyId: 999, isAdmin: false);

        Assert.False(result);
        var vacancy = await dbContext.Vacancies.FirstOrDefaultAsync(v => v.Id == vacancyId);
        Assert.NotNull(vacancy);
        Assert.False(vacancy.IsDeleted);
    }
}