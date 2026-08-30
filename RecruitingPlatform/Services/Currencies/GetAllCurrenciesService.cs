using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Currencies
{
    public class GetAllCurrenciesService (
        RecruitingPlatformDbContext _dbContext)
        : IGetAllCurrenciesService
    {
        public async Task<IEnumerable<Currency>> ExecuteAsync()
        {
            return await _dbContext.Currencies.ToListAsync<Currency>();
        }
    }
}
