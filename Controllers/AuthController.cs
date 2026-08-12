using Microsoft.AspNetCore.Mvc;

namespace RecruitingPlatform.Controllers;

public class AuthController : Controller
{
    [HttpGet]
    public IActionResult LogIn()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult SignUp()
    {
        return View();
    }
}
