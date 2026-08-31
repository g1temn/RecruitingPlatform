using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using RecruitingPlatform.Const.JobSeekers;
using RecruitingPlatform.Controllers;
using RecruitingPlatform.DTOs.JobSeekers;
using RecruitingPlatform.Services.JobSeekers;
using RecruitingPlatform.Services.Profile;
using RecruitingPlatform.ViewModels.JobSeekers;
using RecruitingPlatform.ViewModels.Profile;
using System.Security.Claims;
using Xunit;

namespace RecruitingPlatform.UnitTests.Controllers;

public class JobSeekerProfileControllerTests
{
    private readonly Mock<IGetJobSeekerProfileService> _getProfileServiceMock;
    private readonly Mock<IEditJobSeekerProfileService> _editProfileServiceMock;
    private readonly Mock<ILogger<JobSeekerProfileController>> _loggerMock;
    private readonly JobSeekerProfileController _controller;

    public JobSeekerProfileControllerTests()
    {
        _getProfileServiceMock = new Mock<IGetJobSeekerProfileService>();
        _editProfileServiceMock = new Mock<IEditJobSeekerProfileService>();
        _loggerMock = new Mock<ILogger<JobSeekerProfileController>>();

        _controller = new JobSeekerProfileController(
            _getProfileServiceMock.Object,
            _editProfileServiceMock.Object,
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
    public async Task Index_Get_InvalidUserId_ReturnsUnauthorized()
    {
        SetUserContext("invalid_id");

        var result = await _controller.Index();

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Index_Get_ProfileNotFound_ReturnsNotFound()
    {
        SetUserContext("1");
        _getProfileServiceMock.Setup(s => s.ExecuteAsync(1)).ReturnsAsync((JobSeekerProfileViewModel?)null);

        var result = await _controller.Index();

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Профіль не знайдено або він був видалений.", notFoundResult.Value);
    }

    [Fact]
    public async Task Index_Get_ValidProfile_ReturnsViewWithProfile()
    {
        SetUserContext("1");
        var profile = new JobSeekerProfileViewModel();
        _getProfileServiceMock.Setup(s => s.ExecuteAsync(1)).ReturnsAsync(profile);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(profile, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Get_InvalidUserId_ReturnsUnauthorized()
    {
        SetUserContext("invalid_id");

        var result = await _controller.Edit();

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Edit_Get_ProfileNotFound_ReturnsNotFound()
    {
        SetUserContext("1");
        _editProfileServiceMock.Setup(s => s.GetProfileForEditAsync(1)).ReturnsAsync((EditJobSeekerDto?)null);

        var result = await _controller.Edit();

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(JobSeekerProfileConstants.ProfileNotFoundErrorMessage, notFoundResult.Value);
    }

    [Fact]
    public async Task Edit_Get_ValidData_ReturnsViewWithModel()
    {
        SetUserContext("1");
        var dto = new EditJobSeekerDto();
        _editProfileServiceMock.Setup(s => s.GetProfileForEditAsync(1)).ReturnsAsync(dto);

        var result = await _controller.Edit();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EditJobSeekerViewModel>(viewResult.Model);
        Assert.Equal(dto, model.FormData);
    }

    [Fact]
    public async Task Edit_Post_InvalidUserId_ReturnsUnauthorized()
    {
        SetUserContext("invalid_id");
        var formData = new EditJobSeekerDto();

        var result = await _controller.Edit(formData);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Edit_Post_InvalidModelState_ReturnsViewWithModel()
    {
        SetUserContext("1");
        _controller.ModelState.AddModelError("Error", "Invalid data");
        var formData = new EditJobSeekerDto();

        var result = await _controller.Edit(formData);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EditJobSeekerViewModel>(viewResult.Model);
        Assert.Equal(formData, model.FormData);
    }

    [Fact]
    public async Task Edit_Post_UpdateFails_AddsErrorToModelStateAndReturnsView()
    {
        SetUserContext("1");
        var formData = new EditJobSeekerDto();
        _editProfileServiceMock.Setup(s => s.UpdateProfileAsync(1, formData)).ReturnsAsync(false);

        var result = await _controller.Edit(formData);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Equal(JobSeekerProfileConstants.ProfileUpdateFailedMessage, _controller.ModelState[string.Empty].Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task Edit_Post_Success_SetsTempDataAndRedirectsToEdit()
    {
        SetUserContext("1");
        var formData = new EditJobSeekerDto();
        _editProfileServiceMock.Setup(s => s.UpdateProfileAsync(1, formData)).ReturnsAsync(true);

        var result = await _controller.Edit(formData);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(_controller.Edit), redirectResult.ActionName);
        Assert.Equal(JobSeekerProfileConstants.ProfileUpdatedSuccessMessage, _controller.TempData[JobSeekerProfileConstants.SuccessMessageTempDataKey]);
    }
}