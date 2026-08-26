using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RecruitingPlatform.Controllers;

[Authorize]
public class ProfileController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        if (User.IsInRole("JobSeeker"))
        {
            return RedirectToAction("Index", "JobSeekerProfile");
        }
        else if (User.IsInRole("Company"))
        {
            return RedirectToAction("Index", "CompanyProfile");
        }
        else if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "AdminDashboard");
        }

        return RedirectToAction("Index", "Home");
    }
}