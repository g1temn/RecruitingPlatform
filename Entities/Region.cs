using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecruitingPlatform.Entities;

[Table("regions")]
[Index(nameof(Name), IsUnique = true)]
public class Region
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public IEnumerable<Location> Locations { get; set; } = new List<Location>();
}
