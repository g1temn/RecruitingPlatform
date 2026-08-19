using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecruitingPlatform.Entities
{
    [Table("industries")]
    [Index(nameof(Name), IsUnique = true)]
    public class Industry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public IEnumerable<Specialty> Specialties { get; set; } = new List<Specialty>();
    }
}
