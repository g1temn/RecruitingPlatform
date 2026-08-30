using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitingPlatform.Const.Vacancies;
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
        IGetAllLocationsService _getAllLocationsService,
        IEditVacancyService _editVacancyService,
        IDeleteVacancyService _deleteVacancyService)
        : Controller
    {
        [Authorize(Roles = nameof(PossibleUserRole.JobSeeker) + "," + nameof(PossibleUserRole.Admin))]
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] VacancyFiltersDto filters)
        {
            if (filters.Page < 1) filters.Page = 1;

            var result = await _getVacanciesWithFiltersService.ExecuteAsync(filters);

            ViewBag.CurrentPage = result.CurrentPage;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalItems = result.TotalItems;

            ViewBag.Filters = filters;
            ViewBag.Locations = await _getAllLocationsService.ExecuteAsync();
            ViewBag.Specialties = await _getAllSpecialtiesService.ExecuteAsync();

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

        [Authorize(Roles = nameof(PossibleUserRole.Employer))]
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

        [Authorize(Roles = nameof(PossibleUserRole.Employer))]
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        [Authorize(Roles = $"{nameof(PossibleUserRole.Employer)},{nameof(PossibleUserRole.Admin)}")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int companyId)) return Unauthorized();

            bool isAdmin = User.IsInRole(nameof(PossibleUserRole.Admin));

            var dto = await _editVacancyService.GetForEditAsync(id, companyId, isAdmin);
            if (dto == null) return NotFound(VacanciesConstants.VacancyNotFoundErrorMessage);

            var allSkills = await _getAllSkillsService.ExecuteAsync();
            var viewModel = new EditVacancyViewModel
            {
                Specialties = await _getAllSpecialtiesService.ExecuteAsync(),
                Locations = await _getAllLocationsService.ExecuteAsync(),
                Currencies = await _getAllCurrenciesService.ExecuteAsync(),
                GroupedSkills = allSkills.GroupBy(s => s.SkillType?.Name ?? "Інше").ToDictionary(g => g.Key, g => g.ToList()),
                FormData = dto
            };

            return View(viewModel);
        }

        [Authorize(Roles = $"{nameof(PossibleUserRole.Employer)},{nameof(PossibleUserRole.Admin)}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditVacancyDto formData)
        {
            if (!ModelState.IsValid)
            {
                var allSkills = await _getAllSkillsService.ExecuteAsync();
                var viewModel = new EditVacancyViewModel
                {
                    Specialties = await _getAllSpecialtiesService.ExecuteAsync(),
                    Locations = await _getAllLocationsService.ExecuteAsync(),
                    Currencies = await _getAllCurrenciesService.ExecuteAsync(),
                    GroupedSkills = allSkills.GroupBy(s => s.SkillType?.Name ?? "Інше").ToDictionary(g => g.Key, g => g.ToList()),
                    FormData = formData
                };
                return View(viewModel);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int companyId)) return Unauthorized();

            bool isAdmin = User.IsInRole(nameof(PossibleUserRole.Admin));

            bool isSuccess = await _editVacancyService.UpdateAsync(companyId, formData, isAdmin);

            if (!isSuccess)
            {
                ModelState.AddModelError(string.Empty, VacanciesConstants.VacancyUpdateFailedMessage);
                var allSkills = await _getAllSkillsService.ExecuteAsync();
                var viewModel = new EditVacancyViewModel
                {
                    Specialties = await _getAllSpecialtiesService.ExecuteAsync(),
                    Locations = await _getAllLocationsService.ExecuteAsync(),
                    Currencies = await _getAllCurrenciesService.ExecuteAsync(),
                    GroupedSkills = allSkills.GroupBy(s => s.SkillType?.Name ?? "Інше").ToDictionary(g => g.Key, g => g.ToList()),
                    FormData = formData
                };
                return View(viewModel);
            }

            TempData[VacanciesConstants.SuccessMessageTempDataKey] = VacanciesConstants.VacancyUpdatedSuccessMessage;
            return RedirectToAction(nameof(Details), new { id = formData.Id });
        }

        [Authorize(Roles = $"{nameof(PossibleUserRole.Employer)},{nameof(PossibleUserRole.Admin)}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int companyId)) return Unauthorized();

            bool isAdmin = User.IsInRole(nameof(PossibleUserRole.Admin));

            bool isSuccess = await _deleteVacancyService.DeleteAsync(id, companyId, isAdmin);

            if (!isSuccess)
            {
                TempData[VacanciesConstants.ErrorMessageTempDataKey] = VacanciesConstants.VacancyDeleteFailedMessage;
                return RedirectToAction(nameof(Details), new { id = id });
            }

            TempData[VacanciesConstants.SuccessMessageTempDataKey] = VacanciesConstants.VacancyDeletedSuccessMessage;

            if (isAdmin)
            {
                return RedirectToAction("Index", "Admin", new { tab = "vacancies" });
            }

            return RedirectToAction("Index", "EmployerProfile");
        }
    }
}