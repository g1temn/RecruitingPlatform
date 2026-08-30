using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Skills;

public interface IGetAllSkillsService
{
    Task<IEnumerable<Skill>> ExecuteAsync();
}