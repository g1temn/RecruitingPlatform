using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Applications;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Applications;

public class GetApplicationForReviewServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAsync_ApplicationBelongsToEmployer_ReturnsApplication()
    {
        using var dbContext = GetInMemoryDbContext();

        var specialty = new Specialty { Id = 1, Name = "Test" };
        var company = new Company { Id = 5, Name = "Company" };
        var jobSeeker = new JobSeeker { Id = 100, FirstName = "A", LastName = "B" };
        var status = new ApplicationStatus { Id = 30, Name = "Applied" };

        var vacancy = new Vacancy { Id = 10, CompanyId = 5, Company = company, SpecialtyId = 1, Specialty = specialty };
        var resume = new Resume { Id = 20, JobSeekerId = 100, JobSeeker = jobSeeker, SpecialtyId = 1, Specialty = specialty };

        dbContext.Specialties.Add(specialty);
        dbContext.Companies.Add(company);
        dbContext.JobSeekers.Add(jobSeeker);
        dbContext.ApplicationStatuses.Add(status);
        dbContext.Vacancies.Add(vacancy);
        dbContext.Resumes.Add(resume);

        var application = new Application
        {
            Id = 1,
            VacancyId = 10,
            Vacancy = vacancy,
            ResumeId = 20,
            Resume = resume,
            ApplicationStatusId = 30,
            ApplicationStatus = status
        };

        dbContext.Applications.Add(application);
        await dbContext.SaveChangesAsync();

        var service = new GetApplicationForReviewService(dbContext);

        var result = await service.ExecuteAsync(1, 5);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task ExecuteAsync_ApplicationBelongsToDifferentEmployer_ReturnsNull()
    {
        using var dbContext = GetInMemoryDbContext();

        var specialty = new Specialty { Id = 1, Name = "Test" };
        var company = new Company { Id = 99, Name = "Company" };
        var jobSeeker = new JobSeeker { Id = 100, FirstName = "A", LastName = "B" };
        var status = new ApplicationStatus { Id = 30, Name = "Applied" };

        var vacancy = new Vacancy { Id = 20, CompanyId = 99, Company = company, SpecialtyId = 1, Specialty = specialty };
        var resume = new Resume { Id = 20, JobSeekerId = 100, JobSeeker = jobSeeker, SpecialtyId = 1, Specialty = specialty };

        dbContext.Specialties.Add(specialty);
        dbContext.Companies.Add(company);
        dbContext.JobSeekers.Add(jobSeeker);
        dbContext.ApplicationStatuses.Add(status);
        dbContext.Vacancies.Add(vacancy);
        dbContext.Resumes.Add(resume);

        var application = new Application
        {
            Id = 2,
            VacancyId = 20,
            Vacancy = vacancy,
            ResumeId = 20,
            Resume = resume,
            ApplicationStatusId = 30,
            ApplicationStatus = status
        };

        dbContext.Applications.Add(application);
        await dbContext.SaveChangesAsync();

        var service = new GetApplicationForReviewService(dbContext);

        var result = await service.ExecuteAsync(2, 5);

        Assert.Null(result);
    }

    [Fact]
    public async Task ExecuteAsync_ApplicationDoesNotExist_ReturnsNull()
    {
        using var dbContext = GetInMemoryDbContext();
        var service = new GetApplicationForReviewService(dbContext);

        var result = await service.ExecuteAsync(999, 5);

        Assert.Null(result);
    }
}