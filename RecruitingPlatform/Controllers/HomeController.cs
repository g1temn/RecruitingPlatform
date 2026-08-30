using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RecruitingPlatform.DTOs.Error;
using RecruitingPlatform.Enums;
using System.Diagnostics;

namespace RecruitingPlatform.Controllers
{
    public class HomeController(
        ILogger<HomeController> _logger)
        : Controller
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
                    return RedirectToAction("Index", "Admin");  
            }
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            if (exceptionHandlerPathFeature?.Error != null)
            {
                _logger.LogError(
                    exceptionHandlerPathFeature.Error,
                    "Error address: {Path}. Request ID: {RequestId}. Details: {Message}",
                    exceptionHandlerPathFeature.Path,
                    requestId,
                    exceptionHandlerPathFeature.Error.Message);
            }

            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
