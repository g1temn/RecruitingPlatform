using Microsoft.AspNetCore.Identity;
using RecruitingPlatform.Data;
using RecruitingPlatform.DTOs.Auth;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Enums;

namespace RecruitingPlatform.Services.Auth;

public class SignJobSeekerUpService(
    UserManager<User> userManager,
    RecruitingPlatformDbContext dbContext)
    : SignUpBaseService(userManager), ISignJobSeekerUpService
{
    public async Task<bool> ExecuteAsync(SignJobSeekerUpDto dto)
    {
        var user = await CreateUserWithRoleAsync(dto.Email, dto.Password, PossibleUserRole.JobSeeker);
        if (user == null) return false;

        var jobSeeker = new JobSeeker
        {
            Id = user.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            ContactPhone = dto.ContactPhone,
            ContactEmail = dto.ContactEmail,
            Birthday = dto.Birthday,
            IsDeleted = false
        };

        await dbContext.JobSeekers.AddAsync(jobSeeker);
        await dbContext.SaveChangesAsync();

        return true;
    }
}