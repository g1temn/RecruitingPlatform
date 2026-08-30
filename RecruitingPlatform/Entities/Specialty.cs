using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingPlatform.Entities;

[Table("specialties")]
[Index(nameof(Name), nameof(IndustryId), IsUnique = true)]
public class Specialty
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int IndustryId { get; set; }

    [ForeignKey(nameof(IndustryId))]
    public Industry Industry { get; set; } = null!;
}
