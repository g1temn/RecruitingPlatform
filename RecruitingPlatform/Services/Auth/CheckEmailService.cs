using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Auth;

public class CheckEmailExistsService (
    RecruitingPlatformDbContext _dbContext)
    : ICheckEmailExsistsService
{
    public async Task<bool> ExecuteAcync(string email)
    {
        var result = await _dbContext.Users.FirstOrDefaultAsync<User>(x => x.Email == email);
        if (result == null) return false;
        return true;
    }
}
