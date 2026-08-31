using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Profile;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Profile;

public class GetJobSeekerProfileServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAsync_JobSeekerExistsAndNotDeleted_ReturnsViewModel()
    {
        using var dbContext = GetInMemoryDbContext();

        var specialty = new Specialty { Id = 1 };
        var company = new Company { Id = 1 };
        var vacancy = new Vacancy { Id = 10, CompanyId = 1, Company = company, SpecialtyId = 1, Specialty = specialty };
        var jobSeeker = new JobSeeker { Id = 100, FirstName = "First", IsDeleted = false };
        var resume = new Resume { Id = 20, JobSeekerId = 100, JobSeeker = jobSeeker, SpecialtyId = 1, Specialty = specialty, IsDeleted = false };
        var status = new ApplicationStatus { Id = 30 };
        var application = new Application { Id = 1000, VacancyId = 10, Vacancy = vacancy, ResumeId = 20, Resume = resume, ApplicationStatusId = 30, ApplicationStatus = status };

        dbContext.Specialties.Add(specialty);
        dbContext.Companies.Add(company);
        dbContext.JobSeekers.Add(jobSeeker);
        dbContext.Vacancies.Add(vacancy);
        dbContext.Resumes.Add(resume);
        dbContext.ApplicationStatuses.Add(status);
        dbContext.Applications.Add(application);
        await dbContext.SaveChangesAsync();

        var service = new GetJobSeekerProfileService(dbContext);

        var result = await service.ExecuteAsync(100);

        Assert.NotNull(result);
        Assert.Equal(100, result.Id);
        Assert.Equal("First", result.FirstName);
        Assert.Single(result.Resumes);
        Assert.Single(result.Applications);
        Assert.Equal(1000, result.Applications.First().Id);
    }

    [Fact]
    public async Task ExecuteAsync_JobSeekerDoesNotExistOrDeleted_ReturnsNull()
    {
        using var dbContext = GetInMemoryDbContext();
        dbContext.JobSeekers.Add(new JobSeeker { Id = 1, IsDeleted = true });
        await dbContext.SaveChangesAsync();

        var service = new GetJobSeekerProfileService(dbContext);

        var result = await service.ExecuteAsync(1);

        Assert.Null(result);
    }
}