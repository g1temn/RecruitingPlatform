using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingPlatform.Entities;

[Table("skill_types")]
[Index(nameof(Name), IsUnique = true)]
public class SkillType
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public IEnumerable<Skill> Skills { get; set; } = new List<Skill>();
}
