using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Resumes;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Resumes;

public class DeleteResumeServiceTests : DbTestBase
{
    [Fact]
    public async Task DeleteAsync_ResumeBelongsToUser_SoftDeletesAndReturnsTrue()
    {
        using var dbContext = GetInMemoryDbContext();
        var resumeId = 201;
        var resume = new Resume { Id = resumeId, JobSeekerId = 100, Title = "Title", Summary = "Sum", IsDeleted = false, CreatedAt = DateTime.UtcNow };
        dbContext.Resumes.Add(resume);
        await dbContext.SaveChangesAsync();

        var service = new DeleteResumeService(dbContext);
        var result = await service.DeleteAsync(resumeId, 100);

        Assert.True(result);

        var deletedResume = await dbContext.Resumes.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Id == resumeId);
        Assert.NotNull(deletedResume);
        Assert.True(deletedResume.IsDeleted);
    }

    [Fact]
    public async Task DeleteAsync_UserIsAdmin_SoftDeletesAndReturnsTrue()
    {
        using var dbContext = GetInMemoryDbContext();
        var resumeId = 202;
        var resume = new Resume { Id = resumeId, JobSeekerId = 100, Title = "Title", Summary = "Sum", IsDeleted = false, CreatedAt = DateTime.UtcNow };
        dbContext.Resumes.Add(resume);
        await dbContext.SaveChangesAsync();

        var service = new DeleteResumeService(dbContext);
        var result = await service.DeleteAsync(resumeId, 999, isAdmin: true);

        Assert.True(result);
        var deletedResume = await dbContext.Resumes.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Id == resumeId);
        Assert.NotNull(deletedResume);
        Assert.True(deletedResume.IsDeleted);
    }

    [Fact]
    public async Task DeleteAsync_ResumeBelongsToDifferentUser_ReturnsFalse()
    {
        using var dbContext = GetInMemoryDbContext();
        var resumeId = 203;
        var resume = new Resume { Id = resumeId, JobSeekerId = 100, Title = "Title", Summary = "Sum", IsDeleted = false, CreatedAt = DateTime.UtcNow };
        dbContext.Resumes.Add(resume);
        await dbContext.SaveChangesAsync();

        var service = new DeleteResumeService(dbContext);
        var result = await service.DeleteAsync(resumeId, 999, isAdmin: false);

        Assert.False(result);
        var unchangedResume = await dbContext.Resumes.FirstOrDefaultAsync(r => r.Id == resumeId);
        Assert.NotNull(unchangedResume);
        Assert.False(unchangedResume.IsDeleted);
    }
}