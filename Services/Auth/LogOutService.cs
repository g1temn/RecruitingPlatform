using Microsoft.AspNetCore.Identity;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Auth;

public class LogOutService (
    SignInManager<User> _signInManager)
    : ILogOutService
{
    public async Task ExecuteAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
