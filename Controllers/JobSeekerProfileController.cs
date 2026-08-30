using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecruitingPlatform.Const.JobSeekers;
using RecruitingPlatform.DTOs.JobSeekers;
using RecruitingPlatform.Services.JobSeekers;
using RecruitingPlatform.Services.Profile;
using RecruitingPlatform.ViewModels.JobSeekers;
using System.Security.Claims;

namespace RecruitingPlatform.Controllers;

[Authorize(Roles = "JobSeeker")]
public class JobSeekerProfileController(
    IGetJobSeekerProfileService _getJobSeekerProfileService,
    IEditJobSeekerProfileService _editProfileService,
    ILogger<JobSeekerProfileController> _logger)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
            _logger.LogWarning("Failed to parse job seeker ID from claims in profile Index.");
            return Unauthorized();
        }

        var profile = await _getJobSeekerProfileService.ExecuteAsync(userId);

        if (profile == null)
        {
            _logger.LogWarning("Job seeker profile not found for user ID: {UserId}", userId);
            return NotFound("Профіль не знайдено або він був видалений.");
        }

        return View(profile);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int jobSeekerId))
        {
            _logger.LogWarning("Failed to parse job seeker ID from claims in profile Edit GET.");
            return Unauthorized();
        }

        var dto = await _editProfileService.GetProfileForEditAsync(jobSeekerId);

        if (dto == null)
        {
            _logger.LogWarning("Job seeker profile for edit not found for ID: {JobSeekerId}", jobSeekerId);
            return NotFound(JobSeekerProfileConstants.ProfileNotFoundErrorMessage);
        }

        var viewModel = new EditJobSeekerViewModel { FormData = dto };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditJobSeekerDto formData)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int jobSeekerId))
        {
            _logger.LogWarning("Failed to parse job seeker ID from claims in profile Edit POST.");
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state submitted during job seeker profile edit for ID: {JobSeekerId}", jobSeekerId);
            var viewModel = new EditJobSeekerViewModel { FormData = formData };
            return View(viewModel);
        }

        bool isSuccess = await _editProfileService.UpdateProfileAsync(jobSeekerId, formData);

        if (!isSuccess)
        {
            _logger.LogError("Failed to update job seeker profile for ID: {JobSeekerId}", jobSeekerId);
            ModelState.AddModelError(string.Empty, JobSeekerProfileConstants.ProfileUpdateFailedMessage);
            var viewModel = new EditJobSeekerViewModel { FormData = formData };
            return View(viewModel);
        }

        _logger.LogInformation("Successfully updated job seeker profile for ID: {JobSeekerId}", jobSeekerId);
        TempData[JobSeekerProfileConstants.SuccessMessageTempDataKey] = JobSeekerProfileConstants.ProfileUpdatedSuccessMessage;

        return RedirectToAction(nameof(Edit));
    }
}