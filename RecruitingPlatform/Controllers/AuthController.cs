using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecruitingPlatform.Const.Auth;
using RecruitingPlatform.DTOs.Auth;
using RecruitingPlatform.Enums;
using RecruitingPlatform.Services.Auth;

namespace RecruitingPlatform.Controllers;

public class AuthController(
    ILogInService _logInService,
    ILogOutService _logOutService,
    ISignEmployerUpService _signEmployerUpService,
    ISignJobSeekerUpService _signJobSeekerUpService,
    ICheckEmailExsistsService _checkEmailExsistsService,
    ILogger<AuthController> _logger)
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
            _logger.LogWarning("Login attempt failed due to invalid model state for email: {Email}", dto.Email);
            return View(dto);
        }

        var success = await _logInService.ExecuteAsync(dto);
        if (success)
        {
            _logger.LogInformation("User successfully logged in: {Email}", dto.Email);
            return RedirectToAction("Index", "Home");
        }

        _logger.LogWarning("Failed login attempt for email: {Email}", dto.Email);
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
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Signup step 1 failed due to invalid model state.");
            return View();
        }

        if (dto.Password != dto.ConfirmPassword)
        {
            _logger.LogWarning("Signup failed. Passwords do not match for email: {Email}", dto.Email);
            ModelState.AddModelError(string.Empty, AuthConstants.PasswordsDoNotMatch);
            return View(dto);
        }

        if (await _checkEmailExsistsService.ExecuteAcync(dto.Email))
        {
            _logger.LogWarning("Signup failed. Email already exists: {Email}", dto.Email);
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

        _logger.LogWarning("Signup failed. Invalid role selected for email: {Email}", dto.Email);
        ModelState.AddModelError(string.Empty, AuthConstants.InvalidRoleSelected);
        return View(dto);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignJobSeekerUp(SignJobSeekerUpDto dto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Job seeker signup step 2 failed due to invalid model state for email: {Email}", dto.Email);
            ViewBag.BaseData = dto as SignUpBaseDto;
            return View(dto);
        }

        var success = await _signJobSeekerUpService.ExecuteAsync(dto);
        if (success)
        {
            _logger.LogInformation("Successfully registered new Job Seeker: {Email}", dto.Email);
            return RedirectToAction("LogIn", "Auth");
        }

        _logger.LogError("Error occurred while registering Job Seeker: {Email}", dto.Email);
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
            _logger.LogWarning("Employer signup step 2 failed due to invalid model state for email: {Email}", dto.Email);
            ViewBag.BaseData = dto as SignUpBaseDto;
            return View(dto);
        }

        var success = await _signEmployerUpService.ExecuteAsync(dto);
        if (success)
        {
            _logger.LogInformation("Successfully registered new Employer: {Email}", dto.Email);
            return RedirectToAction("LogIn", "Auth");
        }

        _logger.LogError("Error occurred while registering Employer: {Email}", dto.Email);
        ModelState.AddModelError(string.Empty, AuthConstants.EmployerRegistrationError);
        ViewBag.BaseData = dto as SignUpBaseDto;
        return View(dto);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> LogOut()
    {
        var userName = User.Identity?.Name ?? "Unknown";
        await _logOutService.ExecuteAsync();
        _logger.LogInformation("User logged out: {User}", userName);
        return RedirectToAction("Index", "Home");
    }
}