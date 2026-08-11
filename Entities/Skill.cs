using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingPlatform.Entities;

[Table("skills")]
[Index(nameof(Name), IsUnique = true)]
public class Skill
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int SkillTypeId { get; set; }

    [Required]
    public bool IsHardSkill { get; set; } = true;

    [ForeignKey(nameof(SkillTypeId))]
    public SkillType SkillType { get; set; } = null!;

    public IEnumerable<ResumeSkill> CandidateSkills { get; set; } = new List<ResumeSkill>();
    public IEnumerable<VacancySkill> VacancySkills { get; set; } = new List<VacancySkill>();
}

