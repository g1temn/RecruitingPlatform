using System.ComponentModel.DataAnnotations;
using RecruitingPlatform.Const.JobSeekers;

namespace RecruitingPlatform.DTOs.JobSeekers;

public class EditJobSeekerDto
{
    [Required(ErrorMessage = JobSeekerProfileConstants.FirstNameRequired)]
    [MaxLength(100, ErrorMessage = JobSeekerProfileConstants.FirstNameMaxLength)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = JobSeekerProfileConstants.LastNameRequired)]
    [MaxLength(100, ErrorMessage = JobSeekerProfileConstants.LastNameMaxLength)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = JobSeekerProfileConstants.PhoneRequired)]
    [MaxLength(20, ErrorMessage = JobSeekerProfileConstants.PhoneMaxLength)]
    public string ContactPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = JobSeekerProfileConstants.EmailRequired)]
    [EmailAddress(ErrorMessage = JobSeekerProfileConstants.EmailInvalidFormat)]
    [MaxLength(255, ErrorMessage = JobSeekerProfileConstants.EmailMaxLength)]
    public string ContactEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = JobSeekerProfileConstants.BirthdayRequired)]
    [DataType(DataType.Date)]
    public DateOnly Birthday { get; set; }
}