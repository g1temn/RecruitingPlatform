using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitingPlatform.DTOs.Applications;
using RecruitingPlatform.Services.Applications;
using RecruitingPlatform.Services.Resumes;
using RecruitingPlatform.Services.Vacancies;
using RecruitingPlatform.ViewModels.Applications;
using System.Security.Claims;

namespace RecruitingPlatform.Controllers;

[Authorize(Roles = "JobSeeker")]
public class ApplicationsController(
    ICreateApplicationService _createApplicationService,
    IGetActiveResumesByJobSeekerIdService _getActiveResumesService,
    IGetVacancyByIdService _getVacancyByIdService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Apply(int vacancyId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int jobSeekerId)) return Unauthorized();

        var userResumes = await _getActiveResumesService.ExecuteAsync(jobSeekerId);

        if (!userResumes.Any())
        {
            TempData["Message"] = "Для подачі заявки спочатку створіть хоча б одне резюме.";
            return RedirectToAction("Create", "Resumes");
        }

        var vacancy = await _getVacancyByIdService.ExecuteAsync(vacancyId);
        if (vacancy == null) return View("VacancyNotFound");

        var viewModel = new ApplyViewModel
        {
            Vacancy = vacancy,
            UserResumes = userResumes,
            FormData = new ApplyForVacancyDto { VacancyId = vacancyId }
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Apply(ApplyForVacancyDto formData)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Apply", new { vacancyId = formData.VacancyId });
        }

        var success = await _createApplicationService.ExecuteAsync(formData);

        if (!success)
        {
            TempData["ErrorMessage"] = "Ви вже відправляли це резюме на дану вакансію.";
            return RedirectToAction("Apply", new { vacancyId = formData.VacancyId });
        }

        TempData["SuccessMessage"] = "Вашу заявку успішно відправлено!";
        return RedirectToAction("Details", "Vacancies", new { id = formData.VacancyId });
    }
}