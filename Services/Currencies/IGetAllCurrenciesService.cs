using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Currencies;

public interface IGetAllCurrenciesService
{
    Task<IEnumerable<Currency>> ExecuteAsync();
}
