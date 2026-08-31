using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RecruitingPlatform.Controllers;
using RecruitingPlatform.DTOs.Auth;
using RecruitingPlatform.Enums;
using RecruitingPlatform.Services.Auth;
using System.Security.Claims;

namespace RecruitingPlatform.UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<ILogInService> _logInServiceMock;
    private readonly Mock<ILogOutService> _logOutServiceMock;
    private readonly Mock<ISignEmployerUpService> _signEmployerUpServiceMock;
    private readonly Mock<ISignJobSeekerUpService> _signJobSeekerUpServiceMock;
    private readonly Mock<ICheckEmailExsistsService> _checkEmailExistsServiceMock;
    private readonly Mock<ILogger<AuthController>> _loggerMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _logInServiceMock = new Mock<ILogInService>();
        _logOutServiceMock = new Mock<ILogOutService>();
        _signEmployerUpServiceMock = new Mock<ISignEmployerUpService>();
        _signJobSeekerUpServiceMock = new Mock<ISignJobSeekerUpService>();
        _checkEmailExistsServiceMock = new Mock<ICheckEmailExsistsService>();
        _loggerMock = new Mock<ILogger<AuthController>>();

        _controller = new AuthController(
            _logInServiceMock.Object,
            _logOutServiceMock.Object,
            _signEmployerUpServiceMock.Object,
            _signJobSeekerUpServiceMock.Object,
            _checkEmailExistsServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task LogIn_Post_ValidModelAndSuccess_RedirectsToHome()
    {
        var dto = new LogInDto { Email = "test@test.com", Password = "Password123" };
        _logInServiceMock.Setup(s => s.ExecuteAsync(dto)).ReturnsAsync(true);

        var result = await _controller.LogIn(dto);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    [Fact]
    public async Task LogIn_Post_InvalidModel_ReturnsViewWithDto()
    {
        _controller.ModelState.AddModelError("Email", "Required");
        var dto = new LogInDto();

        var result = await _controller.LogIn(dto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(dto, viewResult.Model);
    }

    [Fact]
    public async Task SignUp_Post_PasswordsDoNotMatch_ReturnsViewWithError()
    {
        var dto = new SignUpBaseDto { Password = "123", ConfirmPassword = "321" };

        var result = await _controller.SignUp(dto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.True(_controller.ModelState.ContainsKey(string.Empty));
    }

    [Fact]
    public async Task SignUp_Post_EmailAlreadyExists_ReturnsViewWithError()
    {
        var dto = new SignUpBaseDto { Email = "exist@test.com", Password = "123", ConfirmPassword = "123" };
        _checkEmailExistsServiceMock.Setup(s => s.ExecuteAcync(dto.Email)).ReturnsAsync(true);

        var result = await _controller.SignUp(dto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task SignUp_Post_ValidJobSeeker_ReturnsJobSeekerSignUpView()
    {
        var dto = new SignUpBaseDto { Email = "new@test.com", Password = "123", ConfirmPassword = "123", Role = PossibleUserRole.JobSeeker };
        _checkEmailExistsServiceMock.Setup(s => s.ExecuteAcync(dto.Email)).ReturnsAsync(false);

        var result = await _controller.SignUp(dto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("SignJobSeekerUp", viewResult.ViewName);
        Assert.Equal(dto, _controller.ViewBag.BaseData);
    }

    [Fact]
    public async Task LogOut_Post_ExecutesLogOutAndRedirectsToHome()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "TestUser") }));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.LogOut();

        _logOutServiceMock.Verify(s => s.ExecuteAsync(), Times.Once);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }
}