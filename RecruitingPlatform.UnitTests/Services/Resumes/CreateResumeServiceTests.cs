using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.DTOs.Resumes;
using RecruitingPlatform.Services.Resumes;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Resumes;

public class CreateResumeServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAsync_WithoutSkills_CreatesResumeAndReturnsId()
    {
        using var dbContext = GetInMemoryDbContext();
        var service = new CreateResumeService(dbContext);
        var dto = new CreateResumeDto
        {
            SpecialtyId = 1,
            Title = "Title",
            Summary = "Summary",
            SelectedSkillIds = new List<int>()
        };

        var result = await service.ExecuteAsync(dto, 100);

        Assert.True(result > 0);
        var savedResume = await dbContext.Resumes.Include(r => r.ResumeSkills).FirstOrDefaultAsync(r => r.Id == result);
        Assert.NotNull(savedResume);
        Assert.Equal(100, savedResume.JobSeekerId);
        Assert.Equal(1, savedResume.SpecialtyId);
        Assert.Empty(savedResume.ResumeSkills);
    }

    [Fact]
    public async Task ExecuteAsync_WithSkills_CreatesResumeWithSkillsAndReturnsId()
    {
        using var dbContext = GetInMemoryDbContext();
        var service = new CreateResumeService(dbContext);
        var dto = new CreateResumeDto
        {
            SpecialtyId = 1,
            Title = "Title",
            Summary = "Summary",
            SelectedSkillIds = new List<int> { 10, 20 }
        };

        var result = await service.ExecuteAsync(dto, 100);

        var savedResume = await dbContext.Resumes.Include(r => r.ResumeSkills).FirstOrDefaultAsync(r => r.Id == result);
        Assert.NotNull(savedResume);
        Assert.Equal(2, savedResume.ResumeSkills.Count());
        Assert.Contains(savedResume.ResumeSkills, rs => rs.SkillId == 10);
        Assert.Contains(savedResume.ResumeSkills, rs => rs.SkillId == 20);
    }
}