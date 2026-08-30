using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using RecruitingPlatform.Const.Resumes;
using RecruitingPlatform.Controllers;
using RecruitingPlatform.DTOs.Common;
using RecruitingPlatform.DTOs.Resumes;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Enums;
using RecruitingPlatform.Services.Resumes;
using RecruitingPlatform.Services.Skills;
using RecruitingPlatform.Services.Specialties;
using RecruitingPlatform.ViewModels.Resumes;
using System.Security.Claims;
using Xunit;

namespace RecruitingPlatform.UnitTests.Controllers;

public class ResumesControllerTests
{
    private readonly Mock<IGetResumeByIdService> _getResumeByIdServiceMock;
    private readonly Mock<IGetResumesWithFiltersService> _getResumesWithFiltersServiceMock;
    private readonly Mock<IGetAllSpecialtiesService> _getAllSpecialtiesServiceMock;
    private readonly Mock<IGetAllSkillsService> _getAllSkillsServiceMock;
    private readonly Mock<ICreateResumeService> _createResumeServiceMock;
    private readonly Mock<IEditResumeService> _editResumeServiceMock;
    private readonly Mock<IDeleteResumeService> _deleteResumeServiceMock;
    private readonly Mock<ILogger<ResumesController>> _loggerMock;
    private readonly ResumesController _controller;

    public ResumesControllerTests()
    {
        _getResumeByIdServiceMock = new Mock<IGetResumeByIdService>();
        _getResumesWithFiltersServiceMock = new Mock<IGetResumesWithFiltersService>();
        _getAllSpecialtiesServiceMock = new Mock<IGetAllSpecialtiesService>();
        _getAllSkillsServiceMock = new Mock<IGetAllSkillsService>();
        _createResumeServiceMock = new Mock<ICreateResumeService>();
        _editResumeServiceMock = new Mock<IEditResumeService>();
        _deleteResumeServiceMock = new Mock<IDeleteResumeService>();
        _loggerMock = new Mock<ILogger<ResumesController>>();

        _controller = new ResumesController(
            _getResumeByIdServiceMock.Object,
            _getResumesWithFiltersServiceMock.Object,
            _getAllSpecialtiesServiceMock.Object,
            _getAllSkillsServiceMock.Object,
            _createResumeServiceMock.Object,
            _editResumeServiceMock.Object,
            _deleteResumeServiceMock.Object,
            _loggerMock.Object)
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
        };
    }

    private void SetUserContext(string userId, string role = "JobSeeker")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, role)
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task Index_Get_SetsViewBagAndReturnsView()
    {
        SetUserContext("1");
        var filters = new ResumeFiltersDto { Page = 0 };

        var pagedResult = new PagedResultDto<Resume>
        {
            CurrentPage = 1,
            TotalPages = 1,
            TotalItems = 0,
            Items = new List<Resume>()
        };

        _getResumesWithFiltersServiceMock
            .Setup(s => s.ExecuteAsync(filters))
            .ReturnsAsync(pagedResult);

        var specialties = new List<Specialty>();
        _getAllSpecialtiesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(specialties);

        var result = await _controller.Index(filters);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(1, filters.Page);
        Assert.Equal(specialties, _controller.ViewBag.Specialties);
        Assert.Equal(filters, _controller.ViewBag.Filters);
    }

    [Fact]
    public async Task Details_Get_ResumeNotFound_ReturnsResumeNotFoundView()
    {
        SetUserContext("1");
        _getResumeByIdServiceMock.Setup(s => s.ExecuteAsync(1)).ReturnsAsync((Resume?)null);

        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("ResumeNotFound", viewResult.ViewName);
    }

    [Fact]
    public async Task Details_Get_ValidResume_ReturnsViewWithResume()
    {
        SetUserContext("1");
        var resume = new Resume { Id = 1 };
        _getResumeByIdServiceMock.Setup(s => s.ExecuteAsync(1)).ReturnsAsync(resume);

        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(resume, viewResult.Model);
    }

    [Fact]
    public async Task Create_Get_ReturnsViewWithGroupedSkillsAndSpecialties()
    {
        SetUserContext("1");
        var specialties = new List<Specialty> { new Specialty() };
        var skills = new List<Skill>
        {
            new Skill { SkillType = new SkillType { Name = "Type1" } },
            new Skill { SkillType = null }
        };

        _getAllSpecialtiesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(specialties);
        _getAllSkillsServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(skills);

        var result = await _controller.Create();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CreateResumeViewModel>(viewResult.Model);

        Assert.Equal(specialties, model.Specialties);
        Assert.True(model.GroupedSkills.ContainsKey("Type1"));
        Assert.True(model.GroupedSkills.ContainsKey("Інше"));
    }

    [Fact]
    public async Task Create_Post_InvalidModelState_ReturnsViewWithModel()
    {
        SetUserContext("1");
        _controller.ModelState.AddModelError("Error", "Invalid data");
        var formData = new CreateResumeDto();

        _getAllSpecialtiesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Specialty>());
        _getAllSkillsServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Skill>());

        var result = await _controller.Create(formData);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CreateResumeViewModel>(viewResult.Model);
        Assert.Equal(formData, model.FormData);
    }

    [Fact]
    public async Task Create_Post_ValidModel_RedirectsToDetails()
    {
        SetUserContext("1");
        var formData = new CreateResumeDto();
        _createResumeServiceMock.Setup(s => s.ExecuteAsync(formData, 1)).ReturnsAsync(99);

        var result = await _controller.Create(formData);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectResult.ActionName);
        Assert.Equal(99, redirectResult.RouteValues["id"]);
        Assert.Equal(ResumesConstants.ResumeCreatedSuccessMessage, _controller.TempData[ResumesConstants.SuccessMessageTempDataKey]);
    }

    [Fact]
    public async Task Edit_Get_ResumeNotFound_ReturnsNotFound()
    {
        SetUserContext("1");
        _editResumeServiceMock.Setup(s => s.GetForEditAsync(1, 1, false)).ReturnsAsync((EditResumeDto?)null);

        var result = await _controller.Edit(1);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(ResumesConstants.ResumeNotFoundErrorMessage, notFoundResult.Value);
    }

    [Fact]
    public async Task Edit_Get_ValidResume_ReturnsViewWithModel()
    {
        SetUserContext("1", nameof(PossibleUserRole.Admin));
        var dto = new EditResumeDto();

        _editResumeServiceMock.Setup(s => s.GetForEditAsync(1, 1, true)).ReturnsAsync(dto);
        _getAllSpecialtiesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Specialty>());
        _getAllSkillsServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Skill>());

        var result = await _controller.Edit(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EditResumeViewModel>(viewResult.Model);
        Assert.Equal(dto, model.FormData);
    }

    [Fact]
    public async Task Edit_Post_InvalidModelState_ReturnsViewWithModel()
    {
        SetUserContext("1");
        _controller.ModelState.AddModelError("Error", "Invalid");
        var formData = new EditResumeDto { Id = 1 };

        _getAllSpecialtiesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Specialty>());
        _getAllSkillsServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Skill>());

        var result = await _controller.Edit(formData);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EditResumeViewModel>(viewResult.Model);
        Assert.Equal(formData, model.FormData);
    }

    [Fact]
    public async Task Edit_Post_UpdateFails_AddsErrorAndReturnsView()
    {
        SetUserContext("1");
        var formData = new EditResumeDto { Id = 1 };
        _editResumeServiceMock.Setup(s => s.UpdateAsync(1, formData, false)).ReturnsAsync(false);
        _getAllSpecialtiesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Specialty>());
        _getAllSkillsServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Skill>());

        var result = await _controller.Edit(formData);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Edit_Post_Success_RedirectsToDetails()
    {
        SetUserContext("1");
        var formData = new EditResumeDto { Id = 10 };
        _editResumeServiceMock.Setup(s => s.UpdateAsync(1, formData, false)).ReturnsAsync(true);

        var result = await _controller.Edit(formData);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectResult.ActionName);
        Assert.Equal(10, redirectResult.RouteValues["id"]);
    }

    [Fact]
    public async Task Delete_Post_Fails_RedirectsToDetails()
    {
        SetUserContext("1");
        _deleteResumeServiceMock.Setup(s => s.DeleteAsync(1, 1, false)).ReturnsAsync(false);

        var result = await _controller.Delete(1);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectResult.ActionName);
        Assert.Equal(1, redirectResult.RouteValues["id"]);
    }

    [Fact]
    public async Task Delete_Post_SuccessAdmin_RedirectsToAdmin()
    {
        SetUserContext("1", nameof(PossibleUserRole.Admin));
        _deleteResumeServiceMock.Setup(s => s.DeleteAsync(1, 1, true)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Admin", redirectResult.ControllerName);
    }

    [Fact]
    public async Task Delete_Post_SuccessJobSeeker_RedirectsToProfile()
    {
        SetUserContext("1", nameof(PossibleUserRole.JobSeeker));
        _deleteResumeServiceMock.Setup(s => s.DeleteAsync(1, 1, false)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Profile", redirectResult.ControllerName);
    }
}