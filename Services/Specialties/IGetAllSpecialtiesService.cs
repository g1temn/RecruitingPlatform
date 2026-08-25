using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Services.Specialties;

public interface IGetAllSpecialtiesService
{
    Task<IEnumerable<Specialty>> ExecuteAsync();
}