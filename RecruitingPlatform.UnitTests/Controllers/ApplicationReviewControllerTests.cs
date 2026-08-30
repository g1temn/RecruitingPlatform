using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using RecruitingPlatform.Const.Application;
using RecruitingPlatform.Controllers;
using RecruitingPlatform.DTOs.Applications;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Applications;
using RecruitingPlatform.ViewModels.Applications;
using System.Security.Claims;
using Xunit;

namespace RecruitingPlatform.UnitTests.Controllers;

public class ApplicationReviewControllerTests
{
    private readonly Mock<IGetApplicationForReviewService> _getAppServiceMock;
    private readonly Mock<IGetAllApplicationStatusesService> _getStatusesServiceMock;
    private readonly Mock<IUpdateApplicationStatusService> _updateStatusServiceMock;
    private readonly Mock<ILogger<ApplicationReviewController>> _loggerMock;
    private readonly ApplicationReviewController _controller;

    public ApplicationReviewControllerTests()
    {
        _getAppServiceMock = new Mock<IGetApplicationForReviewService>();
        _getStatusesServiceMock = new Mock<IGetAllApplicationStatusesService>();
        _updateStatusServiceMock = new Mock<IUpdateApplicationStatusService>();
        _loggerMock = new Mock<ILogger<ApplicationReviewController>>();

        _controller = new ApplicationReviewController(
            _getAppServiceMock.Object,
            _getStatusesServiceMock.Object,
            _updateStatusServiceMock.Object,
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
    public async Task Review_Get_InvalidUserId_ReturnsUnauthorized()
    {
        SetUserContext("invalid_id");
        var result = await _controller.Review(1);
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Review_Get_ApplicationNotFound_ReturnsNotFound()
    {
        SetUserContext("1");
        _getAppServiceMock.Setup(s => s.ExecuteAsync(1, 1)).ReturnsAsync((Application)null);

        var result = await _controller.Review(1);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(ApplicationConstants.CouldNotFindApplication, notFoundResult.Value);
    }

    [Fact]
    public async Task Review_Get_ValidData_ReturnsViewWithModel()
    {
        SetUserContext("1");
        var appEntity = new Application { Id = 1, ApplicationStatusId = 2 };
        var statuses = new List<ApplicationStatus>();

        _getAppServiceMock.Setup(s => s.ExecuteAsync(1, 1)).ReturnsAsync(appEntity);
        _getStatusesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(statuses);

        var result = await _controller.Review(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ReviewApplicationViewModel>(viewResult.Model);

        Assert.Equal(appEntity, model.Application);
        Assert.Equal(statuses, model.AvailableStatuses);
    }

    [Fact]
    public async Task Review_Post_InvalidModelState_RedirectsToReview()
    {
        SetUserContext("1");
        _controller.ModelState.AddModelError("Error", "Invalid data");
        var formData = new UpdateApplicationStatusDto { ApplicationId = 10 };

        var result = await _controller.Review(formData);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Review", redirectResult.ActionName);
        Assert.Equal(10, redirectResult.RouteValues["id"]);
    }

    [Fact]
    public async Task Review_Post_UpdateFails_SetsErrorTempDataAndRedirects()
    {
        SetUserContext("1");
        var formData = new UpdateApplicationStatusDto { ApplicationId = 10 };
        _updateStatusServiceMock.Setup(s => s.ExecuteAsync(formData, 1)).ReturnsAsync(false);

        var result = await _controller.Review(formData);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Review", redirectResult.ActionName);
        Assert.Equal(ApplicationConstants.ApplicationUpdateError, _controller.TempData["ErrorMessage"]);
    }

    [Fact]
    public async Task Review_Post_UpdateSucceeds_SetsSuccessTempDataAndRedirects()
    {
        SetUserContext("1");
        var formData = new UpdateApplicationStatusDto { ApplicationId = 10 };
        _updateStatusServiceMock.Setup(s => s.ExecuteAsync(formData, 1)).ReturnsAsync(true);

        var result = await _controller.Review(formData);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Review", redirectResult.ActionName);
        Assert.Equal(ApplicationConstants.SuccessfulApplicationUpdate, _controller.TempData["SuccessMessage"]);
    }
}