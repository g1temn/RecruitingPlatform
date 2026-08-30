using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RecruitingPlatform.Controllers;
using RecruitingPlatform.DTOs.Error;
using RecruitingPlatform.Enums;
using System.Diagnostics;
using System.Security.Claims;
using Xunit;

namespace RecruitingPlatform.UnitTests.Controllers;

public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _loggerMock;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _loggerMock = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(_loggerMock.Object);
    }

    private void SetUserContext(bool isAuthenticated, string role = null)
    {
        var claims = new List<Claim>();
        if (role != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, isAuthenticated ? "TestAuthType" : null);
        var user = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public void Index_Get_UnauthenticatedUser_ReturnsViewResult()
    {
        SetUserContext(isAuthenticated: false);

        var result = _controller.Index();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Index_Get_JobSeeker_RedirectsToVacanciesIndex()
    {
        SetUserContext(isAuthenticated: true, role: nameof(PossibleUserRole.JobSeeker));

        var result = _controller.Index();

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Vacancies", redirectResult.ControllerName);
    }

    [Fact]
    public void Index_Get_Employer_RedirectsToResumesIndex()
    {
        SetUserContext(isAuthenticated: true, role: nameof(PossibleUserRole.Employer));

        var result = _controller.Index();

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Resumes", redirectResult.ControllerName);
    }

    [Fact]
    public void Index_Get_Admin_RedirectsToAdminIndex()
    {
        SetUserContext(isAuthenticated: true, role: nameof(PossibleUserRole.Admin));

        var result = _controller.Index();

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Admin", redirectResult.ControllerName);
    }

    [Fact]
    public void Error_Get_WithoutExceptionFeature_ReturnsErrorViewWithRequestId()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "test-trace-id";
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = _controller.Error();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);

        Assert.Equal("test-trace-id", model.RequestId);
    }

    [Fact]
    public void Error_Get_WithExceptionFeature_ReturnsErrorViewAndLogsError()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "test-trace-id";

        var expectedException = new Exception("Test exception message");
        var exceptionFeatureMock = new Mock<IExceptionHandlerPathFeature>();
        exceptionFeatureMock.Setup(f => f.Error).Returns(expectedException);
        exceptionFeatureMock.Setup(f => f.Path).Returns("/faulty-route");

        httpContext.Features.Set<IExceptionHandlerPathFeature>(exceptionFeatureMock.Object);
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = _controller.Error();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
        Assert.Equal("test-trace-id", model.RequestId);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("/faulty-route")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}