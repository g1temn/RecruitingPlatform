using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Applications;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Applications;

public class GetAllApplicationStatusesServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAsync_DatabaseIsEmpty_ReturnsEmptyCollection()
    {
        using var dbContext = GetInMemoryDbContext();
        var service = new GetAllApplicationStatusesService(dbContext);

        var result = await service.ExecuteAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteAsync_DatabaseHasStatuses_ReturnsAllStatuses()
    {
        using var dbContext = GetInMemoryDbContext();

        dbContext.ApplicationStatuses.AddRange(
            new ApplicationStatus { Id = 1, Name = "Applied" },
            new ApplicationStatus { Id = 2, Name = "Reviewing" },
            new ApplicationStatus { Id = 3, Name = "Rejected" }
        );
        await dbContext.SaveChangesAsync();

        var service = new GetAllApplicationStatusesService(dbContext);

        var result = await service.ExecuteAsync();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, s => s.Name == "Reviewing");
    }
}