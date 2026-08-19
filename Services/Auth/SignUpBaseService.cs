using Microsoft.AspNetCore.Identity;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Enums;

namespace RecruitingPlatform.Services.Auth;

public abstract class SignUpBaseService(UserManager<User> _userManager)
{
    protected async Task<User?> CreateUserWithRoleAsync(
        string email,
        string password,
        PossibleUserRole role)
    {
        var user = new User
        {
            UserName = email.Split('@')[0],
            Email = email,
            RefreshToken = string.Empty,
            RefreshTokenExpiry = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded) return null;

        await _userManager.AddToRoleAsync(user, role.ToString());

        return user;
    }
}
