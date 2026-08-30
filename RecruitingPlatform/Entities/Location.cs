using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingPlatform.Entities;

[Table("locations")]
[Index(nameof(City), nameof(RegionId), IsUnique = true)]
public class Location
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    public int RegionId { get; set; }

    [ForeignKey(nameof(RegionId))]
    public Region Region { get; set; } = null!;

    public IEnumerable<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
}
