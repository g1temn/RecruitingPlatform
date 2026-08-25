using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitingPlatform.Enums;
using RecruitingPlatform.Services.Resumes;
using RecruitingPlatform.DTOs.Resumes;

namespace RecruitingPlatform.Controllers
{
    public class ResumesController(
        IGetResumeByIdService _getResumeByIdService,
        IGetResumesWithFiltersService _getResumesWithFiltersService)
        : Controller
    {
        [Authorize(Roles = nameof(PossibleUserRole.Employer) + "," + nameof(PossibleUserRole.Admin))]
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ResumeFiltersDto filters)
        {
            if (filters.Page < 1) filters.Page = 1;

            var result = await _getResumesWithFiltersService.ExecuteAsync(filters);

            ViewBag.CurrentPage = result.CurrentPage;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalItems = result.TotalItems;
            ViewBag.Filters = filters;

            return View(result.Items);
        }

        [Authorize(Roles = nameof(PossibleUserRole.Employer) + "," + nameof(PossibleUserRole.Admin))]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var resume = await _getResumeByIdService.ExecuteAsync(id);
            if (resume == null) return View("ResumeNotFound");
            return View(resume);
        }
    }
}
