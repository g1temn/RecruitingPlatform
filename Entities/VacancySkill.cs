using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingPlatform.Entities;

[Table("vacancies_skills")]
[PrimaryKey(nameof(SkillId), nameof(VacancyId))]
public class VacancySkill
{
    public int SkillId { get; set; }

    public int VacancyId { get; set; }

    [ForeignKey(nameof(SkillId))]
    public Skill Skill { get; set; } = null!;

    [ForeignKey(nameof(VacancyId))]
    public Vacancy Vacancy { get; set; } = null!;
}
