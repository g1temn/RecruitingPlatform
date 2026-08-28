using RecruitingPlatform.Data;
using RecruitingPlatform.DTOs.Resumes;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Resumes;

public class CreateResumeService(RecruitingPlatformDbContext _dbContext) : ICreateResumeService
{
    public async Task<int> ExecuteAsync(CreateResumeDto dto, int jobSeekerId)
    {
        var resume = new Resume
        {
            JobSeekerId = jobSeekerId,
            SpecialtyId = dto.SpecialtyId,
            Title = dto.Title,
            Summary = dto.Summary,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        if (dto.SelectedSkillIds != null && dto.SelectedSkillIds.Any())
        {
            resume.ResumeSkills = dto.SelectedSkillIds.Select(skillId => new ResumeSkill
            {
                SkillId = skillId
            }).ToList();
        }

        _dbContext.Resumes.Add(resume);
        await _dbContext.SaveChangesAsync();

        return resume.Id;
    }
}