using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Specialties;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Specialties;

public class GetAllSpecialtiesServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAsync_ReturnsAllSpecialtiesOrderedByName()
    {
        using var dbContext = GetInMemoryDbContext();

        dbContext.Specialties.AddRange(
            new Specialty { Id = 1, Name = "Zebra Specialty" },
            new Specialty { Id = 2, Name = "Alpha Specialty" },
            new Specialty { Id = 3, Name = "Beta Specialty" }
        );
        await dbContext.SaveChangesAsync();

        var service = new GetAllSpecialtiesService(dbContext);

        var result = await service.ExecuteAsync();

        Assert.NotNull(result);
        var list = result.ToList();
        Assert.Equal(3, list.Count);
        Assert.Equal("Alpha Specialty", list[0].Name);
        Assert.Equal("Beta Specialty", list[1].Name);
        Assert.Equal("Zebra Specialty", list[2].Name);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoSpecialties_ReturnsEmptyList()
    {
        using var dbContext = GetInMemoryDbContext();
        var service = new GetAllSpecialtiesService(dbContext);

        var result = await service.ExecuteAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}