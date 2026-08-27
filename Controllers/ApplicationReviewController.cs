using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitingPlatform.DTOs.Applications;
using RecruitingPlatform.Services.Applications;
using RecruitingPlatform.ViewModels.Applications;
using System.Security.Claims;
using RecruitingPlatform.Const.Application;

namespace RecruitingPlatform.Controllers;

[Authorize(Roles = "Employer")]
public class ApplicationReviewController (
    IGetApplicationForReviewService _getApplicationForReviewService,
    IGetAllApplicationStatusesService _getAllApplicationStatusesService,
    IUpdateApplicationStatusService _updateApplicationStatusService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Review(int id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int employerId)) return Unauthorized();

        var application = await _getApplicationForReviewService.ExecuteAsync(id, employerId);

        if (application == null)
        {
            return NotFound(ApplicationConstants.CouldNotFindApplication);
        }

        var statuses = await _getAllApplicationStatusesService.ExecuteAsync();

        var viewModel = new ReviewApplicationViewModel
        {
            Application = application,
            AvailableStatuses = statuses,
            FormData = new UpdateApplicationStatusDto
            {
                ApplicationId = application.Id,
                NewStatusId = application.ApplicationStatusId
            }
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Review(UpdateApplicationStatusDto formData)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int employerId)) return Unauthorized();

        if (!ModelState.IsValid)
        {
            return RedirectToAction("Review", new { id = formData.ApplicationId });
        }

        var success = await _updateApplicationStatusService.ExecuteAsync(formData, employerId);

        if (success)
        {
            TempData["SuccessMessage"] = ApplicationConstants.SuccessfulApplicationUpdate;
        }
        else
        {
            TempData["ErrorMessage"] = ApplicationConstants.ApplicationUpdateError;
        }

        return RedirectToAction("Review", new { id = formData.ApplicationId });
    }
}
