using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Locations;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Locations;

public class GetAllLocationsServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAsync_DatabaseIsEmpty_ReturnsEmptyCollection()
    {
        using var dbContext = GetInMemoryDbContext();
        var service = new GetAllLocationsService(dbContext);

        var result = await service.ExecuteAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteAsync_DatabaseHasLocations_ReturnsAllLocationsOrderedByCity()
    {
        using var dbContext = GetInMemoryDbContext();

        dbContext.Locations.AddRange(
            new Location { Id = 1, City = "Львів" },
            new Location { Id = 2, City = "Запоріжжя" },
            new Location { Id = 3, City = "Київ" }
        );
        await dbContext.SaveChangesAsync();

        var service = new GetAllLocationsService(dbContext);

        var result = await service.ExecuteAsync();
        var resultList = result.ToList();

        Assert.NotNull(resultList);
        Assert.Equal(3, resultList.Count);
        Assert.Equal("Запоріжжя", resultList[0].City);
        Assert.Equal("Київ", resultList[1].City);
        Assert.Equal("Львів", resultList[2].City);
    }
}