using Microsoft.AspNetCore.Identity;
using RecruitingPlatform.Data;
using RecruitingPlatform.DTOs.Auth;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Enums;
using System.Runtime.InteropServices;

namespace RecruitingPlatform.Services.Auth;

public class SignEmployerUpService (
    UserManager<User> _userManager, 
    RecruitingPlatformDbContext _dbContext
    ) : SignUpBaseService (_userManager), ISignEmployerUpService
{
    public async Task<bool> ExecuteAsync(SignEmployerUpDto dto)
    {
        var user = await CreateUserWithRoleAsync(dto.Email, dto.Password, dto.Role);
        if (user == null) return false;

        var company = new Company
        {
            Id = user.Id,
            Name = dto.Name,
            WebsiteUrl = dto.WebsiteUrl,
            ContactPhone = dto.ContactPhone,
            Description = dto.Description,
            IsDeleted = false
        };

        await _dbContext.Companies.AddAsync(company);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}
