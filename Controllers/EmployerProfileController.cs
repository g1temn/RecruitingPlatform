using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitingPlatform.Services.Profile;
using System.Security.Claims;

namespace RecruitingPlatform.Controllers;

[Authorize(Roles = "Employer")]
public class EmployerProfileController(
    IGetEmployerProfileService _getEmployerProfileService)
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
}