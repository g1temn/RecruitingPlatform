using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.DTOs.Employers;

namespace RecruitingPlatform.Services.Employers;

public class EditEmployerProfileService(RecruitingPlatformDbContext _dbContext) : IEditEmployerProfileService
{
    public async Task<EditEmployerDto?> GetProfileForEditAsync(int companyId)
    {
        var company = await _dbContext.Companies
            .FirstOrDefaultAsync(c => c.Id == companyId && !c.IsDeleted);

        if (company == null) return null;

        return new EditEmployerDto
        {
            Name = company.Name,
            Description = company.Description,
            ContactPhone = company.ContactPhone,
            WebsiteUrl = company.WebsiteUrl
        };
    }

    public async Task<bool> UpdateProfileAsync(int companyId, EditEmployerDto dto)
    {
        var company = await _dbContext.Companies
            .FirstOrDefaultAsync(c => c.Id == companyId && !c.IsDeleted);

        if (company == null)
            return false;

        company.Name = dto.Name;
        company.Description = dto.Description;
        company.ContactPhone = dto.ContactPhone;
        company.WebsiteUrl = dto.WebsiteUrl;

        await _dbContext.SaveChangesAsync();

        return true;
    }
}