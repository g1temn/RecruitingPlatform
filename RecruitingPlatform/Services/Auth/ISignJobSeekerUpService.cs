using RecruitingPlatform.DTOs.Auth;

namespace RecruitingPlatform.Services.Auth;

public interface ISignJobSeekerUpService
{
    Task<bool> ExecuteAsync(SignJobSeekerUpDto dto);
}