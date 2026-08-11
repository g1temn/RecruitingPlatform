using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.Mime.MediaTypeNames;

namespace RecruitingPlatform.Entities;

[Table("vacancies")]
public class Vacancy
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public int SpecialtyId { get; set; }

    public int? LocationId { get; set; }

    [Required]
    public bool IsRemote { get; set; } = false;

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? MinSalary { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? MaxSalary { get; set; }

    [Required]
    public int SalaryCurrencyId { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public bool IsDeleted { get; set; } = false;

    [ForeignKey(nameof(CompanyId))]
    public Company Company { get; set; } = null!;

    [ForeignKey(nameof(SpecialtyId))]
    public Specialty Specialty { get; set; } = null!;

    [ForeignKey(nameof(LocationId))]
    public Location Location { get; set; } = null!;

    [ForeignKey(nameof(SalaryCurrencyId))]
    public Currency Currency { get; set; } = null!;

    public ICollection<VacancySkill> VacancySkills { get; set; } = new List<VacancySkill>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}