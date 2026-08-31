using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Resumes;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Resumes;

public class GetResumeByIdServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAsync_ResumeExistsAndNotDeleted_ReturnsResumeWithIncludes()
    {
        using var dbContext = GetInMemoryDbContext();
        var jobSeeker = new JobSeeker { Id = 100, FirstName = "First" };
        var specialty = new Specialty { Id = 10, Name = "Spec" };
        var skill = new Skill { Id = 1, Name = "Skill1" };

        var resume = new Resume
        {
            Id = 1,
            JobSeekerId = 100,
            JobSeeker = jobSeeker,
            SpecialtyId = 10,
            Specialty = specialty,
            IsDeleted = false,
            ResumeSkills = new List<ResumeSkill>
            {
                new ResumeSkill { SkillId = 1, Skill = skill }
            }
        };

        dbContext.JobSeekers.Add(jobSeeker);
        dbContext.Specialties.Add(specialty);
        dbContext.Skills.Add(skill);
        dbContext.Resumes.Add(resume);
        await dbContext.SaveChangesAsync();

        var service = new GetResumeByIdService(dbContext);

        var result = await service.ExecuteAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.NotNull(result.JobSeeker);
        Assert.NotNull(result.Specialty);
        Assert.Single(result.ResumeSkills);
        Assert.NotNull(result.ResumeSkills.First().Skill);
    }

    [Fact]
    public async Task ExecuteAsync_ResumeIsDeleted_ReturnsNull()
    {
        using var dbContext = GetInMemoryDbContext();
        dbContext.Resumes.Add(new Resume { Id = 1, IsDeleted = true });
        await dbContext.SaveChangesAsync();

        var service = new GetResumeByIdService(dbContext);

        var result = await service.ExecuteAsync(1);

        Assert.Null(result);
    }
}