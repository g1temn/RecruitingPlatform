using Microsoft.Extensions.DependencyInjection;
using RecruitingPlatform.Services.Applications;
using RecruitingPlatform.Services.Auth;
using RecruitingPlatform.Services.Currencies;
using RecruitingPlatform.Services.Employers;
using RecruitingPlatform.Services.JobSeekers;
using RecruitingPlatform.Services.Locations;
using RecruitingPlatform.Services.Profile;
using RecruitingPlatform.Services.Resumes;
using RecruitingPlatform.Services.Skills;
using RecruitingPlatform.Services.Specialties;
using RecruitingPlatform.Services.Vacancies;

namespace RecruitingPlatform.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        // Auth Services
        services.AddScoped<ILogInService, LogInService>();
        services.AddScoped<ILogOutService, LogOutService>();
        services.AddScoped<ISignJobSeekerUpService, SignJobSeekerUpService>();
        services.AddScoped<ISignEmployerUpService, SignEmployerUpService>();
        services.AddScoped<ICheckEmailExsistsService, CheckEmailExistsService>();

        // Profile Services
        services.AddScoped<IGetJobSeekerProfileService, GetJobSeekerProfileService>();
        services.AddScoped<IGetEmployerProfileService, GetEmployerProfileService>();
        services.AddScoped<IEditJobSeekerProfileService, EditJobSeekerProfileService>();
        services.AddScoped<IEditEmployerProfileService, EditEmployerProfileService>();

        // Resume Services
        services.AddScoped<IGetResumesWithFiltersService, GetResumesWithFiltersService>();
        services.AddScoped<IGetResumeByIdService, GetResumeByIdService>();
        services.AddScoped<IGetActiveResumesByJobSeekerIdService, GetActiveResumesByJobSeekerIdService>();
        services.AddScoped<ICreateResumeService, CreateResumeService>();
        services.AddScoped<IEditResumeService, EditResumeService>();
        services.AddScoped<IDeleteResumeService, DeleteResumeService>();

        // Vacancy Services
        services.AddScoped<IGetVacanciesWithFiltersService, GetVacanciesWithFiltersService>();
        services.AddScoped<IGetVacancyByIdService, GetVacancyByIdService>();
        services.AddScoped<ICreateVacancyService, CreateVacancyService>();
        services.AddScoped<IEditVacancyService, EditVacancyService>();
        services.AddScoped<IDeleteVacancyService, DeleteVacancyService>();

        // Application Services
        services.AddScoped<ICreateApplicationService, CreateApplicationService>();
        services.AddScoped<IGetAllApplicationStatusesService, GetAllApplicationStatusesService>();
        services.AddScoped<IGetApplicationForReviewService, GetApplicationForReviewService>();
        services.AddScoped<IUpdateApplicationStatusService, UpdateApplicationStatusService>();

        // Dictionary/Lookup Services
        services.AddScoped<IGetAllSpecialtiesService, GetAllSpecialtiesService>();
        services.AddScoped<IGetAllSkillsService, GetAllSkillsService>();
        services.AddScoped<IGetAllLocationsService, GetAllLocationsService>();
        services.AddScoped<IGetAllCurrenciesService, GetAllCurrenciesService>();

        return services;
    }
}