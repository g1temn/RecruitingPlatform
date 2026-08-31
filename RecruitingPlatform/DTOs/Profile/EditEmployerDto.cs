using System.ComponentModel.DataAnnotations;
using RecruitingPlatform.Const.Employers;

namespace RecruitingPlatform.DTOs.Employer;

public class EditEmployerDto
{
    [Required(ErrorMessage = EmployerProfileConstants.NameRequired)]
    [MaxLength(100, ErrorMessage = EmployerProfileConstants.NameMaxLength)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [MaxLength(20, ErrorMessage = EmployerProfileConstants.PhoneMaxLength)]
    public string? ContactPhone { get; set; }

    [Url(ErrorMessage = EmployerProfileConstants.WebsiteInvalidFormat)]
    [MaxLength(255, ErrorMessage = EmployerProfileConstants.WebsiteMaxLength)]
    public string? WebsiteUrl { get; set; }
}