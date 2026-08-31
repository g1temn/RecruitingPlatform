using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.DTOs.Employer;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Employers;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Employers;

public class EditEmployerProfileServiceTests : DbTestBase
{
    [Fact]
    public async Task GetProfileForEditAsync_CompanyExistsAndNotDeleted_ReturnsDto()
    {
        using var dbContext = GetInMemoryDbContext();
        dbContext.Companies.Add(new Company
        {
            Id = 1,
            Name = "Test",
            Description = "Desc",
            ContactPhone = "123",
            WebsiteUrl = "url",
            IsDeleted = false
        });
        await dbContext.SaveChangesAsync();

        var service = new EditEmployerProfileService(dbContext);

        var result = await service.GetProfileForEditAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
        Assert.Equal("Desc", result.Description);
        Assert.Equal("123", result.ContactPhone);
        Assert.Equal("url", result.WebsiteUrl);
    }

    [Fact]
    public async Task GetProfileForEditAsync_CompanyDoesNotExistOrDeleted_ReturnsNull()
    {
        using var dbContext = GetInMemoryDbContext();
        dbContext.Companies.Add(new Company { Id = 1, IsDeleted = true });
        await dbContext.SaveChangesAsync();

        var service = new EditEmployerProfileService(dbContext);

        var result = await service.GetProfileForEditAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateProfileAsync_CompanyExistsAndNotDeleted_UpdatesAndReturnsTrue()
    {
        using var dbContext = GetInMemoryDbContext();
        dbContext.Companies.Add(new Company { Id = 1, Name = "Old", IsDeleted = false });
        await dbContext.SaveChangesAsync();

        var service = new EditEmployerProfileService(dbContext);
        var dto = new EditEmployerDto { Name = "New", Description = "Desc", ContactPhone = "123", WebsiteUrl = "url" };

        var result = await service.UpdateProfileAsync(1, dto);

        Assert.True(result);
        var updatedCompany = await dbContext.Companies.FirstAsync();
        Assert.Equal("New", updatedCompany.Name);
        Assert.Equal("Desc", updatedCompany.Description);
        Assert.Equal("123", updatedCompany.ContactPhone);
        Assert.Equal("url", updatedCompany.WebsiteUrl);
    }

    [Fact]
    public async Task UpdateProfileAsync_CompanyDoesNotExistOrDeleted_ReturnsFalse()
    {
        using var dbContext = GetInMemoryDbContext();
        dbContext.Companies.Add(new Company { Id = 1, Name = "Old", IsDeleted = true });
        await dbContext.SaveChangesAsync();

        var service = new EditEmployerProfileService(dbContext);
        var dto = new EditEmployerDto { Name = "New" };

        var result = await service.UpdateProfileAsync(1, dto);

        Assert.False(result);
    }
}