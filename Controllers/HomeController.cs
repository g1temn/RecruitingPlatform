using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitingPlatform.Enums;
using System.Diagnostics;

namespace RecruitingPlatform.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole(nameof(PossibleUserRole.JobSeeker)))
                    return RedirectToAction("Index", "Vacancies");

                else if (User.IsInRole(nameof(PossibleUserRole.Employer)))
                    return RedirectToAction("Index", "Resumes");

                else if (User.IsInRole(nameof(PossibleUserRole.Admin)))
                    return RedirectToAction("Index", "Administration");  
            }
            return View();
        }
    }
}
