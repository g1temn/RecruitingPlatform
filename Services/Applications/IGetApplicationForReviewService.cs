using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Applications;

public interface IGetApplicationForReviewService
{
    Task<Application?> ExecuteAsync(int applicationId, int employerId);
}