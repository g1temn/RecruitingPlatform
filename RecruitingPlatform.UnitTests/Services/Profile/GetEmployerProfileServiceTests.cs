using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Profile;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Profile;

public class GetEmployerProfileServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAsync_EmployerExistsAndNotDeleted_ReturnsViewModel()
    {
        using var dbContext = GetInMemoryDbContext();

        var specialty = new Specialty { Id = 1 };
        var location = new Location { Id = 1 };
        var company = new Company { Id = 1, Name = "Comp", IsDeleted = false };
        var vacancy = new Vacancy { Id = 10, CompanyId = 1, Company = company, SpecialtyId = 1, Specialty = specialty, LocationId = 1, Location = location, IsDeleted = false };
        var jobSeeker = new JobSeeker { Id = 100 };
        var resume = new Resume { Id = 20, JobSeekerId = 100, JobSeeker = jobSeeker, SpecialtyId = 1, Specialty = specialty };
        var status = new ApplicationStatus { Id = 30 };
        var application = new Application { Id = 1000, VacancyId = 10, Vacancy = vacancy, ResumeId = 20, Resume = resume, ApplicationStatusId = 30, ApplicationStatus = status };

        dbContext.Specialties.Add(specialty);
        dbContext.Locations.Add(location);
        dbContext.Companies.Add(company);
        dbContext.JobSeekers.Add(jobSeeker);
        dbContext.Vacancies.Add(vacancy);
        dbContext.Resumes.Add(resume);
        dbContext.ApplicationStatuses.Add(status);
        dbContext.Applications.Add(application);
        await dbContext.SaveChangesAsync();

        var service = new GetEmployerProfileService(dbContext);

        var result = await service.ExecuteAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Comp", result.Name);
        Assert.Single(result.Vacancies);
        Assert.Single(result.RecentApplications);
        Assert.Equal(1000, result.RecentApplications.First().Id);
    }

    [Fact]
    public async Task ExecuteAsync_EmployerDoesNotExistOrDeleted_ReturnsNull()
    {
        using var dbContext = GetInMemoryDbContext();
        dbContext.Companies.Add(new Company { Id = 1, IsDeleted = true });
        await dbContext.SaveChangesAsync();

        var service = new GetEmployerProfileService(dbContext);

        var result = await service.ExecuteAsync(1);

        Assert.Null(result);
    }
}