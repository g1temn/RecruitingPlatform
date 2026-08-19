using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitingPlatform.Const.Auth;
using RecruitingPlatform.DTOs.Auth;
using RecruitingPlatform.Enums;
using RecruitingPlatform.Services.Auth;

namespace RecruitingPlatform.Controllers;

public class AuthController(
    ILogInService _logInService,
    ISignEmployerUpService _signEmployerUpService,
    ISignJobSeekerUpService _signJobSeekerUpService,
    ICheckEmailExsistsService _checkEmailExsistsService)
    : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult LogIn()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> LogIn(LogInDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var success = await _logInService.ExecuteAsync(dto);
        if (success)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, AuthConstants.InvalidLogin);
        return View(dto);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp(SignUpBaseDto dto)
    {
        if (!ModelState.IsValid) return View();

        if (dto.Password != dto.ConfirmPassword)
        {
            ModelState.AddModelError(string.Empty, AuthConstants.PasswordsDoNotMatch);
            return View(dto);
        }

        if (await _checkEmailExsistsService.ExecuteAcync(dto.Email))
        {
            ModelState.AddModelError(string.Empty, AuthConstants.EmailAlreadyExists);
            return View(dto);
        }

        ViewBag.BaseData = dto;

        if (dto.Role == PossibleUserRole.JobSeeker)
        {
            return View("SignJobSeekerUp");
        }
        else if (dto.Role == PossibleUserRole.Employer)
        {
            return View("SignEmployerUp");
        }

        ModelState.AddModelError(string.Empty, AuthConstants.InvalidRoleSelected);
        return View(dto);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignJobSeekerUp(SignJobSeekerUpDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.BaseData = dto as SignUpBaseDto;
            return View(dto);
        }

        var success = await _signJobSeekerUpService.ExecuteAsync(dto);
        if (success)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, AuthConstants.JobSeekerRegistrationError);
        ViewBag.BaseData = dto as SignUpBaseDto;
        return View(dto);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignEmployerUp(SignEmployerUpDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.BaseData = dto as SignUpBaseDto;
            return View(dto);
        }

        var success = await _signEmployerUpService.ExecuteAsync(dto);
        if (success)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, AuthConstants.EmployerRegistrationError);
        ViewBag.BaseData = dto as SignUpBaseDto;
        return View(dto);
    }
}