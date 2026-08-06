using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingPlatform.Entities;

[Table("currencies")]
public class Currency
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(3)]
    public string Name { get; set; } = string.Empty;

    public IEnumerable<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
}
