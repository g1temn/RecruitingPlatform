using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    IGetVacancyByIdService _getVacancyByIdService,
    ILogger<ApplicationsController> _logger) 
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Apply(int vacancyId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int jobSeekerId))
        {
            _logger.LogWarning("Failed to parse job seeker ID from claims for vacancy {VacancyId}.", vacancyId);
            return Unauthorized();
        }

        var userResumes = await _getActiveResumesService.ExecuteAsync(jobSeekerId);

        if (!userResumes.Any())
        {
            _logger.LogInformation("Job seeker {JobSeekerId} attempted to apply for vacancy {VacancyId} but has no resumes.", jobSeekerId, vacancyId);
            TempData["Message"] = "Для подачі заявки спочатку створіть хоча б одне резюме.";
            return RedirectToAction("Create", "Resumes");
        }

        var vacancy = await _getVacancyByIdService.ExecuteAsync(vacancyId);
        if (vacancy == null)
        {
            _logger.LogWarning("Job seeker {JobSeekerId} tried to apply for non-existent vacancy {VacancyId}.", jobSeekerId, vacancyId);
            return View("VacancyNotFound");
        }

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
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int jobSeekerId)) return Unauthorized();

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state submitted by job seeker {JobSeekerId} for vacancy {VacancyId}.", jobSeekerId, formData.VacancyId);
            return RedirectToAction("Apply", new { vacancyId = formData.VacancyId });
        }

        var success = await _createApplicationService.ExecuteAsync(formData);

        if (!success)
        {
            _logger.LogWarning("Job seeker {JobSeekerId} attempted to submit a duplicate application for vacancy {VacancyId}.", jobSeekerId, formData.VacancyId);
            TempData["ErrorMessage"] = "Ви вже відправляли це резюме на дану вакансію.";
            return RedirectToAction("Apply", new { vacancyId = formData.VacancyId });
        }

        _logger.LogInformation("Job seeker {JobSeekerId} successfully applied for vacancy {VacancyId}.", jobSeekerId, formData.VacancyId);
        TempData["SuccessMessage"] = "Вашу заявку успішно відправлено!";
        return RedirectToAction("Details", "Vacancies", new { id = formData.VacancyId });
    }
}