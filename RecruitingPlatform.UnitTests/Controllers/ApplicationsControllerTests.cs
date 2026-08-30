using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using RecruitingPlatform.Controllers;
using RecruitingPlatform.DTOs.Applications;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Applications;
using RecruitingPlatform.Services.Resumes;
using RecruitingPlatform.Services.Vacancies;
using RecruitingPlatform.ViewModels.Applications;
using System.Security.Claims;
using Xunit;

namespace RecruitingPlatform.UnitTests.Controllers;

public class ApplicationsControllerTests
{
    private readonly Mock<ICreateApplicationService> _createAppServiceMock;
    private readonly Mock<IGetActiveResumesByJobSeekerIdService> _getActiveResumesServiceMock;
    private readonly Mock<IGetVacancyByIdService> _getVacancyServiceMock;
    private readonly Mock<ILogger<ApplicationsController>> _loggerMock;
    private readonly ApplicationsController _controller;

    public ApplicationsControllerTests()
    {
        _createAppServiceMock = new Mock<ICreateApplicationService>();
        _getActiveResumesServiceMock = new Mock<IGetActiveResumesByJobSeekerIdService>();
        _getVacancyServiceMock = new Mock<IGetVacancyByIdService>();
        _loggerMock = new Mock<ILogger<ApplicationsController>>();

        _controller = new ApplicationsController(
            _createAppServiceMock.Object,
            _getActiveResumesServiceMock.Object,
            _getVacancyServiceMock.Object,
            _loggerMock.Object)
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
        };
    }

    private void SetUserContext(string userId)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task Apply_Get_NoResumes_RedirectsToCreateResume()
    {
        SetUserContext("1");
        _getActiveResumesServiceMock.Setup(s => s.ExecuteAsync(1)).ReturnsAsync(new List<Resume>());

        var result = await _controller.Apply(10);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Create", redirectResult.ActionName);
        Assert.Equal("Resumes", redirectResult.ControllerName);
        Assert.NotNull(_controller.TempData["Message"]);
    }

    [Fact]
    public async Task Apply_Get_VacancyNotFound_ReturnsVacancyNotFoundView()
    {
        SetUserContext("1");
        _getActiveResumesServiceMock.Setup(s => s.ExecuteAsync(1)).ReturnsAsync(new List<Resume> { new Resume() });
        _getVacancyServiceMock.Setup(s => s.ExecuteAsync(10)).ReturnsAsync((Vacancy)null);

        var result = await _controller.Apply(10);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("VacancyNotFound", viewResult.ViewName);
    }

    [Fact]
    public async Task Apply_Get_ValidVacancyAndResumes_ReturnsViewWithModel()
    {
        SetUserContext("1");
        var resumes = new List<Resume> { new Resume() };
        var vacancy = new Vacancy { Id = 10 };

        _getActiveResumesServiceMock.Setup(s => s.ExecuteAsync(1)).ReturnsAsync(resumes);
        _getVacancyServiceMock.Setup(s => s.ExecuteAsync(10)).ReturnsAsync(vacancy);

        var result = await _controller.Apply(10);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ApplyViewModel>(viewResult.Model);
        Assert.Equal(vacancy, model.Vacancy);
        Assert.Equal(resumes, model.UserResumes);
        Assert.Equal(10, model.FormData.VacancyId);
    }

    [Fact]
    public async Task Apply_Post_CreateFailsDuplicate_RedirectsToApplyWithError()
    {
        SetUserContext("1");
        var formData = new ApplyForVacancyDto { VacancyId = 10 };
        _createAppServiceMock.Setup(s => s.ExecuteAsync(formData)).ReturnsAsync(false);

        var result = await _controller.Apply(formData);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Apply", redirectResult.ActionName);
        Assert.Equal(10, redirectResult.RouteValues["vacancyId"]);
        Assert.NotNull(_controller.TempData["ErrorMessage"]);
    }

    [Fact]
    public async Task Apply_Post_Success_RedirectsToVacancyDetails()
    {
        SetUserContext("1");
        var formData = new ApplyForVacancyDto { VacancyId = 10 };
        _createAppServiceMock.Setup(s => s.ExecuteAsync(formData)).ReturnsAsync(true);

        var result = await _controller.Apply(formData);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectResult.ActionName);
        Assert.Equal("Vacancies", redirectResult.ControllerName);
        Assert.Equal(10, redirectResult.RouteValues["id"]);
        Assert.NotNull(_controller.TempData["SuccessMessage"]);
    }
}