using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitingPlatform.DTOs.Resumes;
using RecruitingPlatform.DTOs.Vacancies;
using RecruitingPlatform.Enums;
using RecruitingPlatform.Services.Resumes;
using RecruitingPlatform.Services.Vacancies;
using RecruitingPlatform.ViewModels.Admin;

namespace RecruitingPlatform.Controllers;

[Authorize(Roles = nameof(PossibleUserRole.Admin))]
public class AdminController(
    IGetResumesWithFiltersService _getResumesService,
    IGetVacanciesWithFiltersService _getVacanciesService)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string tab = "resumes", int page = 1)
    {
        var viewModel = new AdminDashboardViewModel
        {
            ActiveTab = tab,
            CurrentPage = page
        };

        if (tab == "resumes")
        {
            viewModel.Resumes = await _getResumesService.ExecuteAsync(new ResumeFiltersDto { Page = page });
        }
        else if (tab == "vacancies")
        {
            viewModel.Vacancies = await _getVacanciesService.ExecuteAsync(new VacancyFiltersDto { Page = page });
        }

        return View(viewModel);
    }
}