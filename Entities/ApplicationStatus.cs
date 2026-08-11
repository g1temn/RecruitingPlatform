using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingPlatform.Entities;

[Table("application_statuses")]
[Index(nameof(Name), IsUnique = true)]
public class ApplicationStatus
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;

    public IEnumerable<Application> Applications { get; set; } = new List<Application>();
}
