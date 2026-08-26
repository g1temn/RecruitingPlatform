using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitingPlatform.Services.Profile;
using System.Security.Claims;

namespace RecruitingPlatform.Controllers;

[Authorize(Roles = "JobSeeker")]
public class JobSeekerProfileController(IGetJobSeekerProfileService _getJobSeekerProfileService) : Controller
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
}