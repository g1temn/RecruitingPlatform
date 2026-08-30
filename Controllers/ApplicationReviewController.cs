using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecruitingPlatform.DTOs.Applications;
using RecruitingPlatform.Services.Applications;
using RecruitingPlatform.ViewModels.Applications;
using System.Security.Claims;
using RecruitingPlatform.Const.Application;

namespace RecruitingPlatform.Controllers;

[Authorize(Roles = "Employer")]
public class ApplicationReviewController(
    IGetApplicationForReviewService _getApplicationForReviewService,
    IGetAllApplicationStatusesService _getAllApplicationStatusesService,
    IUpdateApplicationStatusService _updateApplicationStatusService,
    ILogger<ApplicationReviewController> _logger) 
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Review(int id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int employerId))
        {
            _logger.LogWarning("Failed to parse employer ID from claims during application review {ApplicationId}.", id);
            return Unauthorized();
        }

        var application = await _getApplicationForReviewService.ExecuteAsync(id, employerId);

        if (application == null)
        {
            _logger.LogWarning("Employer {EmployerId} attempted to review application {ApplicationId}, but it was not found.", employerId, id);
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
        if (!int.TryParse(userIdString, out int employerId))
        {
            _logger.LogWarning("Failed to parse employer ID from claims on post review for application {ApplicationId}.", formData.ApplicationId);
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state submitted by employer {EmployerId} for application {ApplicationId}.", employerId, formData.ApplicationId);
            return RedirectToAction("Review", new { id = formData.ApplicationId });
        }

        var success = await _updateApplicationStatusService.ExecuteAsync(formData, employerId);

        if (success)
        {
            _logger.LogInformation("Employer {EmployerId} successfully updated status for application {ApplicationId}.", employerId, formData.ApplicationId);
            TempData["SuccessMessage"] = ApplicationConstants.SuccessfulApplicationUpdate;
        }
        else
        {
            _logger.LogError("Employer {EmployerId} failed to update status for application {ApplicationId}.", employerId, formData.ApplicationId);
            TempData["ErrorMessage"] = ApplicationConstants.ApplicationUpdateError;
        }

        return RedirectToAction("Review", new { id = formData.ApplicationId });
    }
}