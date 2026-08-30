using Microsoft.AspNetCore.Identity;
using RecruitingPlatform.DTOs.Auth;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Auth;

public class LogInService (
    SignInManager<User> _signInManager)
    : ILogInService
{
     public async Task<bool> ExecuteAsync(LogInDto dto)
     {
        var result = await _signInManager.PasswordSignInAsync(
            dto.Email.Split('@')[0],
            dto.Password,
            isPersistent: true,
            lockoutOnFailure: false);

        return result.Succeeded;
     }
}
