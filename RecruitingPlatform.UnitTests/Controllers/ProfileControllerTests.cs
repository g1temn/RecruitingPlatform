using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitingPlatform.Controllers;
using System.Security.Claims;
using Xunit;

namespace RecruitingPlatform.UnitTests.Controllers;

public class ProfileControllerTests
{
    private readonly ProfileController _controller;

    public ProfileControllerTests()
    {
        _controller = new ProfileController();
    }

    private void SetUserRole(string role)
    {
        var claims = new List<Claim>();
        if (!string.IsNullOrEmpty(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public void Index_Get_JobSeekerRole_RedirectsToJobSeekerProfile()
    {
        SetUserRole("JobSeeker");

        var result = _controller.Index();

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("JobSeekerProfile", redirectResult.ControllerName);
    }

    [Fact]
    public void Index_Get_EmployerRole_RedirectsToEmployerProfile()
    {
        SetUserRole("Employer");

        var result = _controller.Index();

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("EmployerProfile", redirectResult.ControllerName);
    }

    [Fact]
    public void Index_Get_AdminRole_RedirectsToAdminDashboard()
    {
        SetUserRole("Admin");

        var result = _controller.Index();

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("AdminDashboard", redirectResult.ControllerName);
    }

    [Fact]
    public void Index_Get_NoMatchingRole_RedirectsToHome()
    {
        SetUserRole("UnknownRole");

        var result = _controller.Index();

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }
}