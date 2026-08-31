using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.DTOs.JobSeekers;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.JobSeekers;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.JobSeekers;

public class EditJobSeekerProfileServiceTests : DbTestBase
{
    [Fact]
    public async Task GetProfileForEditAsync_JobSeekerExistsAndNotDeleted_ReturnsDto()
    {
        using var dbContext = GetInMemoryDbContext();
        var date = new DateOnly(2000, 1, 1);
        dbContext.JobSeekers.Add(new JobSeeker
        {
            Id = 1,
            FirstName = "First",
            LastName = "Last",
            ContactPhone = "123",
            ContactEmail = "email",
            Birthday = date,
            IsDeleted = false
        });
        await dbContext.SaveChangesAsync();

        var service = new EditJobSeekerProfileService(dbContext);

        var result = await service.GetProfileForEditAsync(1);

        Assert.NotNull(result);
        Assert.Equal("First", result.FirstName);
        Assert.Equal("Last", result.LastName);
        Assert.Equal("123", result.ContactPhone);
        Assert.Equal("email", result.ContactEmail);
        Assert.Equal(date, result.Birthday);
    }

    [Fact]
    public async Task GetProfileForEditAsync_JobSeekerDoesNotExistOrDeleted_ReturnsNull()
    {
        using var dbContext = GetInMemoryDbContext();
        dbContext.JobSeekers.Add(new JobSeeker { Id = 1, IsDeleted = true });
        await dbContext.SaveChangesAsync();

        var service = new EditJobSeekerProfileService(dbContext);

        var result = await service.GetProfileForEditAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateProfileAsync_JobSeekerExistsAndNotDeleted_UpdatesAndReturnsTrue()
    {
        using var dbContext = GetInMemoryDbContext();
        dbContext.JobSeekers.Add(new JobSeeker { Id = 1, FirstName = "Old", IsDeleted = false });
        await dbContext.SaveChangesAsync();

        var service = new EditJobSeekerProfileService(dbContext);
        var date = new DateOnly(2000, 1, 1);
        var dto = new EditJobSeekerDto { FirstName = "New", LastName = "Last", ContactPhone = "123", ContactEmail = "email", Birthday = date };

        var result = await service.UpdateProfileAsync(1, dto);

        Assert.True(result);
        var updatedJobSeeker = await dbContext.JobSeekers.FirstAsync();
        Assert.Equal("New", updatedJobSeeker.FirstName);
        Assert.Equal("Last", updatedJobSeeker.LastName);
        Assert.Equal("123", updatedJobSeeker.ContactPhone);
        Assert.Equal("email", updatedJobSeeker.ContactEmail);
        Assert.Equal(date, updatedJobSeeker.Birthday);
    }

    [Fact]
    public async Task UpdateProfileAsync_JobSeekerDoesNotExistOrDeleted_ReturnsFalse()
    {
        using var dbContext = GetInMemoryDbContext();
        dbContext.JobSeekers.Add(new JobSeeker { Id = 1, FirstName = "Old", IsDeleted = true });
        await dbContext.SaveChangesAsync();

        var service = new EditJobSeekerProfileService(dbContext);
        var dto = new EditJobSeekerDto { FirstName = "New" };

        var result = await service.UpdateProfileAsync(1, dto);

        Assert.False(result);
    }
}