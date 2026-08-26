using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitingPlatform.DTOs.Vacancies;
using RecruitingPlatform.Enums;
using RecruitingPlatform.Services.Currencies;
using RecruitingPlatform.Services.Locations;
using RecruitingPlatform.Services.Skills;
using RecruitingPlatform.Services.Specialties;
using RecruitingPlatform.Services.Vacancies;
using RecruitingPlatform.ViewModels.Vacancies;
using System.Security.Claims;

namespace RecruitingPlatform.Controllers
{
    public class VacanciesController(
        IGetVacanciesWithFiltersService _getVacanciesWithFiltersService,
        IGetVacancyByIdService _getVacancyByIdService,
        IGetAllSkillsService _getAllSkillsService,
        ICreateVacancyService _createVacancyService,
        IGetAllSpecialtiesService _getAllSpecialtiesService,
        IGetAllCurrenciesService _getAllCurrenciesService,
        IGetAllLocationsService _getAllLocationsService
        )
        : Controller
    {
        [Authorize(Roles = nameof(PossibleUserRole.JobSeeker) + "," + nameof(PossibleUserRole.Admin))]
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery]  VacancyFiltersDto filters)
        {
            if (filters.Page < 1) filters.Page = 1;

            var result = await _getVacanciesWithFiltersService.ExecuteAsync(filters);

            ViewBag.CurrentPage = result.CurrentPage;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalItems = result.TotalItems;
            ViewBag.Filters = filters;

            return View(result.Items);
        }

        [Authorize(Roles = nameof(PossibleUserRole.JobSeeker) + "," + nameof(PossibleUserRole.Admin) + "," + nameof(PossibleUserRole.Employer))]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var vacancy = await _getVacancyByIdService.ExecuteAsync(id);
            if (vacancy == null) return View("VacancyNotFound");
            return View(vacancy);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var allSkills = await _getAllSkillsService.ExecuteAsync();

            var viewModel = new CreateVacancyViewModel
            {
                Specialties = await _getAllSpecialtiesService.ExecuteAsync(),
                Locations = await _getAllLocationsService.ExecuteAsync(),
                Currencies = await _getAllCurrenciesService.ExecuteAsync(),
                GroupedSkills = allSkills
                    .GroupBy(s => s.SkillType?.Name ?? "Інше")
                    .ToDictionary(g => g.Key, g => g.ToList()),
                FormData = new CreateVacancyDto()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateVacancyDto formData)
        {
            if (!ModelState.IsValid)
            {
                var allSkills = await _getAllSkillsService.ExecuteAsync();

                var viewModel = new CreateVacancyViewModel
                {
                    Specialties = await _getAllSpecialtiesService.ExecuteAsync(),
                    Locations = await _getAllLocationsService.ExecuteAsync(),
                    Currencies = await _getAllCurrenciesService.ExecuteAsync(),
                    GroupedSkills = allSkills
                        .GroupBy(s => s.SkillType?.Name ?? "Інше")
                        .ToDictionary(g => g.Key, g => g.ToList()),
                    FormData = formData
                };
                return View(viewModel);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int companyId)) return Unauthorized();

            await _createVacancyService.ExecuteAsync(formData, companyId);

            TempData["SuccessMessage"] = "Вакансію успішно створено!";
            return RedirectToAction("Index", "EmployerProfile");
        }

    }
}
