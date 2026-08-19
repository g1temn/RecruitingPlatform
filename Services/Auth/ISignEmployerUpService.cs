using RecruitingPlatform.DTOs.Auth;

namespace RecruitingPlatform.Services.Auth;

public interface ISignEmployerUpService
{
    Task<bool> ExecuteAsync(SignEmployerUpDto dto);
}