using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    IEditEmployerProfileService _editProfileService)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

        var profile = await _getEmployerProfileService.ExecuteAsync(userId);

        if (profile == null)
        {
            return NotFound("Профіль роботодавця не знайдено або він був видалений.");
        }

        return View(profile);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int companyId)) return Unauthorized();

        var dto = await _editProfileService.GetProfileForEditAsync(companyId);

        if (dto == null)
            return NotFound(EmployerProfileConstants.ProfileNotFoundErrorMessage);

        var viewModel = new EditEmployerViewModel { FormData = dto };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditEmployerDto formData)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = new EditEmployerViewModel { FormData = formData };
            return View(viewModel);
        }

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int companyId)) return Unauthorized();

        bool isSuccess = await _editProfileService.UpdateProfileAsync(companyId, formData);

        if (!isSuccess)
        {
            ModelState.AddModelError(string.Empty, EmployerProfileConstants.ProfileUpdateFailedMessage);
            var viewModel = new EditEmployerViewModel { FormData = formData };
            return View(viewModel);
        }

        TempData[EmployerProfileConstants.SuccessMessageTempDataKey] = EmployerProfileConstants.ProfileUpdatedSuccessMessage;

        return RedirectToAction(nameof(Edit));
    }
}