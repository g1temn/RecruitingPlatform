using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.DTOs.Resumes;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Resumes;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Resumes;

public class EditResumeServiceTests : DbTestBase
{
    [Fact]
    public async Task GetForEditAsync_ValidRequest_ReturnsDto()
    {
        using var dbContext = GetInMemoryDbContext();
        var resume = new Resume
        {
            Id = 1,
            JobSeekerId = 100,
            SpecialtyId = 1,
            Title = "Title",
            Summary = "Summary",
            IsDeleted = false,
            ResumeSkills = new List<ResumeSkill> { new ResumeSkill { SkillId = 10 } }
        };
        dbContext.Resumes.Add(resume);
        await dbContext.SaveChangesAsync();

        var service = new EditResumeService(dbContext);

        var result = await service.GetForEditAsync(1, 100);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Title", result.Title);
        Assert.Single(result.SelectedSkillIds!);
        Assert.Equal(10, result.SelectedSkillIds!.First());
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_UpdatesAndReturnsTrue()
    {
        using var dbContext = GetInMemoryDbContext();
        var resume = new Resume
        {
            Id = 1,
            JobSeekerId = 100,
            Title = "Old",
            IsDeleted = false,
            ResumeSkills = new List<ResumeSkill> { new ResumeSkill { SkillId = 10 } }
        };
        dbContext.Resumes.Add(resume);
        await dbContext.SaveChangesAsync();

        var service = new EditResumeService(dbContext);
        var dto = new EditResumeDto
        {
            Id = 1,
            SpecialtyId = 2,
            Title = "New",
            Summary = "New Summary",
            SelectedSkillIds = new List<int> { 20, 30 }
        };

        var result = await service.UpdateAsync(100, dto);

        Assert.True(result);
        var updatedResume = await dbContext.Resumes.Include(r => r.ResumeSkills).FirstOrDefaultAsync(r => r.Id == 1);
        Assert.Equal("New", updatedResume!.Title);
        Assert.Equal(2, updatedResume.ResumeSkills.Count());
        Assert.DoesNotContain(updatedResume.ResumeSkills, rs => rs.SkillId == 10);
        Assert.Contains(updatedResume.ResumeSkills, rs => rs.SkillId == 20);
    }
}