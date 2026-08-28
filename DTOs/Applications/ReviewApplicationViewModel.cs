using RecruitingPlatform.DTOs.Applications;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.ViewModels.Applications;

public class ReviewApplicationViewModel
{
    public Application Application { get; set; } = null!;

    public IEnumerable<ApplicationStatus> AvailableStatuses { get; set; } = new List<ApplicationStatus>();

    public UpdateApplicationStatusDto FormData { get; set; } = new UpdateApplicationStatusDto();
}