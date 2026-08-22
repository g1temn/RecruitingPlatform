using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitingPlatform.DTOs.Vacancies;
using RecruitingPlatform.Enums;
using RecruitingPlatform.Services.Vacancies;

namespace RecruitingPlatform.Controllers
{
    public class VacanciesController (
        IGetVacanciesWithFiltersService _getVacanciesWithFiltersService)
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

            return View(result.Items);
        }
    }
}
