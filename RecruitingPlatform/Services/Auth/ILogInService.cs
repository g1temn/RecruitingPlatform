using RecruitingPlatform.DTOs.Auth;

namespace RecruitingPlatform.Services.Auth;

public interface ILogInService
{
    Task<bool> ExecuteAsync(LogInDto dto);
}
