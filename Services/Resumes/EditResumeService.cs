using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.DTOs.Resumes;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Resumes;

public class EditResumeService(
    RecruitingPlatformDbContext _dbContext)
    : IEditResumeService
{
    public async Task<EditResumeDto?> GetForEditAsync(int resumeId, int jobSeekerId)
    {
        var resume = await _dbContext.Resumes
            .Include(r => r.ResumeSkills)
            .FirstOrDefaultAsync(r => r.Id == resumeId && r.JobSeekerId == jobSeekerId && !r.IsDeleted);

        if (resume == null) return null;

        return new EditResumeDto
        {
            Id = resume.Id,
            SpecialtyId = resume.SpecialtyId,
            Title = resume.Title,
            Summary = resume.Summary,
            SelectedSkillIds = resume.ResumeSkills.Select(rs => rs.SkillId).ToList()
        };
    }

    public async Task<bool> UpdateAsync(int jobSeekerId, EditResumeDto dto)
    {
        var resume = await _dbContext.Resumes
            .Include(r => r.ResumeSkills)
            .FirstOrDefaultAsync(r => r.Id == dto.Id && r.JobSeekerId == jobSeekerId && !r.IsDeleted);

        if (resume == null) return false;

        resume.SpecialtyId = dto.SpecialtyId;
        resume.Title = dto.Title;
        resume.Summary = dto.Summary;

        resume.UpdatedAt = DateTime.UtcNow;

        _dbContext.ResumeSkills.RemoveRange(resume.ResumeSkills);

        if (dto.SelectedSkillIds != null && dto.SelectedSkillIds.Any())
        {
            resume.ResumeSkills = dto.SelectedSkillIds.Select(skillId => new ResumeSkill
            {
                SkillId = skillId
            }).ToList();
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }
}