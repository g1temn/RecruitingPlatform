using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Resumes;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Resumes;

public class GetActiveResumesByJobSeekerIdServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAsync_ReturnsOnlyActiveResumesForJobSeeker()
    {
        using var dbContext = GetInMemoryDbContext();
        dbContext.Resumes.AddRange(
            new Resume { Id = 1, JobSeekerId = 100, IsDeleted = false },
            new Resume { Id = 2, JobSeekerId = 100, IsDeleted = false },
            new Resume { Id = 3, JobSeekerId = 100, IsDeleted = true },
            new Resume { Id = 4, JobSeekerId = 999, IsDeleted = false }
        );
        await dbContext.SaveChangesAsync();

        var service = new GetActiveResumesByJobSeekerIdService(dbContext);

        var result = await service.ExecuteAsync(100);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, r => r.Id == 1);
        Assert.Contains(result, r => r.Id == 2);
    }
}