using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    IEditJobSeekerProfileService _editProfileService)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

        var profile = await _getJobSeekerProfileService.ExecuteAsync(userId);

        if (profile == null)
        {
            return NotFound("Профіль не знайдено або він був видалений.");
        }

        return View(profile);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int jobSeekerId)) return Unauthorized();

        var dto = await _editProfileService.GetProfileForEditAsync(jobSeekerId);

        if (dto == null)
            return NotFound(JobSeekerProfileConstants.ProfileNotFoundErrorMessage);

        var viewModel = new EditJobSeekerViewModel { FormData = dto };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditJobSeekerDto formData)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = new EditJobSeekerViewModel { FormData = formData };
            return View(viewModel);
        }

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int jobSeekerId)) return Unauthorized();

        bool isSuccess = await _editProfileService.UpdateProfileAsync(jobSeekerId, formData);

        if (!isSuccess)
        {
            ModelState.AddModelError(string.Empty, JobSeekerProfileConstants.ProfileUpdateFailedMessage);
            var viewModel = new EditJobSeekerViewModel { FormData = formData };
            return View(viewModel);
        }

        TempData[JobSeekerProfileConstants.SuccessMessageTempDataKey] = JobSeekerProfileConstants.ProfileUpdatedSuccessMessage;

        return RedirectToAction(nameof(Edit));
    }
}