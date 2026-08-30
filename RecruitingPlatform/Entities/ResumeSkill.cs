using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingPlatform.Entities;

[Table("resumes_skills")]
[PrimaryKey(nameof(SkillId), nameof(ResumeId))]
public class ResumeSkill
{
    public int SkillId { get; set; }
    public int ResumeId { get; set; }

    [ForeignKey(nameof(SkillId))]
    public Skill Skill { get; set; } = null!;

    [ForeignKey(nameof(ResumeId))]
    public Resume Resume { get; set; } = null!;
}
