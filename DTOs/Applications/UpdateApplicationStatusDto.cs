using System.ComponentModel.DataAnnotations;
using RecruitingPlatform.Const.Application;

namespace RecruitingPlatform.DTOs.Applications;

public class UpdateApplicationStatusDto
{
    [Required]
    public int ApplicationId { get; set; }

    [Required(ErrorMessage = ApplicationConstants.PleaseChoseNewStatus)]
    public int NewStatusId { get; set; }
}