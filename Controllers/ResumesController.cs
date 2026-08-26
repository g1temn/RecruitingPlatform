using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitingPlatform.DTOs.Resumes;
using RecruitingPlatform.Enums;
using RecruitingPlatform.Services.Resumes;
using RecruitingPlatform.Services.Skills;
using RecruitingPlatform.Services.Specialties;
using RecruitingPlatform.ViewModels.Resumes;

namespace RecruitingPlatform.Controllers
{
    public class ResumesController(
        IGetResumeByIdService _getResumeByIdService,
        IGetResumesWithFiltersService _getResumesWithFiltersService,
        IGetAllSpecialtiesService _getAllSpecialtiesService,
        IGetAllSkillsService _getAllSkillsService)
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

        [Authorize(Roles = nameof(PossibleUserRole.Employer) + "," + nameof(PossibleUserRole.Admin) + "," + nameof(PossibleUserRole.JobSeeker))]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var resume = await _getResumeByIdService.ExecuteAsync(id);
            if (resume == null) return View("ResumeNotFound");
            return View(resume);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var specialties = await _getAllSpecialtiesService.ExecuteAsync();
            var allSkills = await _getAllSkillsService.ExecuteAsync();

            var groupedSkills = allSkills
                .GroupBy(s => s.SkillType?.Name ?? "Інше")
                .ToDictionary(g => g.Key, g => g.ToList());

            var viewModel = new CreateResumeViewModel
            {
                Specialties = specialties,
                GroupedSkills = groupedSkills,
                FormData = new CreateResumeDto()
            };

            return View(viewModel);
        }
    }
}
