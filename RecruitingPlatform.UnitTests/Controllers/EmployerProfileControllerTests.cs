using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using RecruitingPlatform.Const.Employers;
using RecruitingPlatform.Controllers;
using RecruitingPlatform.DTOs.Employer;
using RecruitingPlatform.Services.Employers;
using RecruitingPlatform.Services.Profile;
using RecruitingPlatform.ViewModels.Employers;
using RecruitingPlatform.ViewModels.Profile;
using System.Security.Claims;

namespace RecruitingPlatform.UnitTests.Controllers;

public class EmployerProfileControllerTests
{
    private readonly Mock<IGetEmployerProfileService> _getProfileServiceMock;
    private readonly Mock<IEditEmployerProfileService> _editProfileServiceMock;
    private readonly Mock<ILogger<EmployerProfileController>> _loggerMock;
    private readonly EmployerProfileController _controller;

    public EmployerProfileControllerTests()
    {
        _getProfileServiceMock = new Mock<IGetEmployerProfileService>();
        _editProfileServiceMock = new Mock<IEditEmployerProfileService>();
        _loggerMock = new Mock<ILogger<EmployerProfileController>>();

        _controller = new EmployerProfileController(
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
    public async Task Index_Get_ProfileNotFound_ReturnsNotFound()
    {
        SetUserContext("1");
        _getProfileServiceMock.Setup(s => s.ExecuteAsync(1)).ReturnsAsync((EmployerProfileViewModel?)null);

        var result = await _controller.Index();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Index_Get_ValidProfile_ReturnsViewWithProfile()
    {
        SetUserContext("1");
        var profile = new EmployerProfileViewModel();
        _getProfileServiceMock.Setup(s => s.ExecuteAsync(1)).ReturnsAsync(profile);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(profile, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Get_ProfileNotFound_ReturnsNotFound()
    {
        SetUserContext("1");
        _editProfileServiceMock.Setup(s => s.GetProfileForEditAsync(1)).ReturnsAsync((EditEmployerDto?)null);

        var result = await _controller.Edit();

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(EmployerProfileConstants.ProfileNotFoundErrorMessage, notFoundResult.Value);
    }

    [Fact]
    public async Task Edit_Post_InvalidModelState_ReturnsViewWithModel()
    {
        SetUserContext("1");
        _controller.ModelState.AddModelError("Error", "Invalid data");
        var formData = new EditEmployerDto();

        var result = await _controller.Edit(formData);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EditEmployerViewModel>(viewResult.Model);
        Assert.Equal(formData, model.FormData);
    }

    [Fact]
    public async Task Edit_Post_UpdateFails_AddsErrorToModelStateAndReturnsView()
    {
        SetUserContext("1");
        var formData = new EditEmployerDto();
        _editProfileServiceMock.Setup(s => s.UpdateProfileAsync(1, formData)).ReturnsAsync(false);

        var result = await _controller.Edit(formData);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Equal(EmployerProfileConstants.ProfileUpdateFailedMessage, _controller.ModelState[string.Empty].Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task Edit_Post_Success_SetsTempDataAndRedirectsToEdit()
    {
        SetUserContext("1");
        var formData = new EditEmployerDto();
        _editProfileServiceMock.Setup(s => s.UpdateProfileAsync(1, formData)).ReturnsAsync(true);

        var result = await _controller.Edit(formData);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Edit", redirectResult.ActionName);
        Assert.Equal(EmployerProfileConstants.ProfileUpdatedSuccessMessage, _controller.TempData[EmployerProfileConstants.SuccessMessageTempDataKey]);
    }
}