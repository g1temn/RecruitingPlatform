using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Locations;

public interface IGetAllLocationsService
{
    Task<IEnumerable<Location>> ExecuteAsync();
}
