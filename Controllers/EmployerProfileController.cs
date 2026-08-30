using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecruitingPlatform.Const.Employers;
using RecruitingPlatform.DTOs.Employers;
using RecruitingPlatform.Services.Employers;
using RecruitingPlatform.Services.Profile;
using RecruitingPlatform.ViewModels.Employers;
using System.Security.Claims;

namespace RecruitingPlatform.Controllers;

[Authorize(Roles = "Employer")]
public class EmployerProfileController(
    IGetEmployerProfileService _getEmployerProfileService,
    IEditEmployerProfileService _editProfileService,
    ILogger<EmployerProfileController> _logger)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
            _logger.LogWarning("Failed to parse employer ID from claims in profile Index.");
            return Unauthorized();
        }

        var profile = await _getEmployerProfileService.ExecuteAsync(userId);

        if (profile == null)
        {
            _logger.LogWarning("Employer profile not found for user ID: {UserId}", userId);
            return NotFound("Профіль роботодавця не знайдено або він був видалений.");
        }

        return View(profile);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int companyId))
        {
            _logger.LogWarning("Failed to parse company ID from claims in profile Edit GET.");
            return Unauthorized();
        }

        var dto = await _editProfileService.GetProfileForEditAsync(companyId);

        if (dto == null)
        {
            _logger.LogWarning("Employer profile for edit not found for company ID: {CompanyId}", companyId);
            return NotFound(EmployerProfileConstants.ProfileNotFoundErrorMessage);
        }

        var viewModel = new EditEmployerViewModel { FormData = dto };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditEmployerDto formData)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int companyId))
        {
            _logger.LogWarning("Failed to parse company ID from claims in profile Edit POST.");
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state submitted during employer profile edit for company ID: {CompanyId}", companyId);
            var viewModel = new EditEmployerViewModel { FormData = formData };
            return View(viewModel);
        }

        bool isSuccess = await _editProfileService.UpdateProfileAsync(companyId, formData);

        if (!isSuccess)
        {
            _logger.LogError("Failed to update employer profile for company ID: {CompanyId}", companyId);
            ModelState.AddModelError(string.Empty, EmployerProfileConstants.ProfileUpdateFailedMessage);
            var viewModel = new EditEmployerViewModel { FormData = formData };
            return View(viewModel);
        }

        _logger.LogInformation("Successfully updated employer profile for company ID: {CompanyId}", companyId);
        TempData[EmployerProfileConstants.SuccessMessageTempDataKey] = EmployerProfileConstants.ProfileUpdatedSuccessMessage;

        return RedirectToAction(nameof(Edit));
    }
}