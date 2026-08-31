using RecruitingPlatform.DTOs.Resumes;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Resumes;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Resumes;

public class GetResumesWithFiltersServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAsync_FilterBySpecialty_ReturnsMatchingResumes()
    {
        using var dbContext = GetInMemoryDbContext();

        var jobSeeker = new JobSeeker { Id = 1, FirstName = "John", LastName = "Doe" };
        var specialty = new Specialty { Id = 10, Name = "Developer" };

        dbContext.JobSeekers.Add(jobSeeker);
        dbContext.Specialties.Add(specialty);

        dbContext.Resumes.AddRange(
            new Resume { Id = 1, Title = "A", Summary = "Test", SpecialtyId = 10, JobSeekerId = 1, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Resume { Id = 2, Title = "B", Summary = "Test", SpecialtyId = 20, JobSeekerId = 1, IsDeleted = false, CreatedAt = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var service = new GetResumesWithFiltersService(dbContext);
        var filters = new ResumeFiltersDto { Page = 1, SpecialtyId = 10 };

        var result = await service.ExecuteAsync(filters);

        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(1, result.Items.First().Id);
    }

    [Fact]
    public async Task ExecuteAsync_FilterBySearchQuery_ReturnsMatchingResumes()
    {
        using var dbContext = GetInMemoryDbContext();

        var jobSeeker = new JobSeeker { Id = 2, FirstName = "John", LastName = "Doe" };
        var specialty = new Specialty { Id = 11, Name = "Developer" };

        dbContext.JobSeekers.Add(jobSeeker);
        dbContext.Specialties.Add(specialty);

        dbContext.Resumes.AddRange(
            new Resume { Id = 3, Title = "UniqueTitleSearch", Summary = "text", SpecialtyId = 11, JobSeekerId = 2, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Resume { Id = 4, Title = "Other", Summary = "text", SpecialtyId = 11, JobSeekerId = 2, IsDeleted = false, CreatedAt = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var service = new GetResumesWithFiltersService(dbContext);
        var filters = new ResumeFiltersDto { Page = 1, SearchQuery = "uniquetitle" };

        var result = await service.ExecuteAsync(filters);

        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(3, result.Items.First().Id);
    }

    [Fact]
    public async Task ExecuteAsync_NoFilters_ReturnsAllPaged()
    {
        using var dbContext = GetInMemoryDbContext();

        var jobSeeker = new JobSeeker { Id = 3, FirstName = "Jane", LastName = "Smith" };
        var specialty = new Specialty { Id = 12, Name = "QA" };

        dbContext.JobSeekers.Add(jobSeeker);
        dbContext.Specialties.Add(specialty);

        dbContext.Resumes.AddRange(
            new Resume { Id = 5, Title = "A", Summary = "Test", SpecialtyId = 12, JobSeekerId = 3, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Resume { Id = 6, Title = "B", Summary = "Test", SpecialtyId = 12, JobSeekerId = 3, IsDeleted = false, CreatedAt = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var service = new GetResumesWithFiltersService(dbContext);
        var filters = new ResumeFiltersDto { Page = 1 };

        var result = await service.ExecuteAsync(filters);

        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalItems);
    }
}