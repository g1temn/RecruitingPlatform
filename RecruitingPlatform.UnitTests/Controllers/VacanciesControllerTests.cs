using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using RecruitingPlatform.Const.Vacancies;
using RecruitingPlatform.Controllers;
using RecruitingPlatform.DTOs.Common;
using RecruitingPlatform.DTOs.Vacancies;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Enums;
using RecruitingPlatform.Services.Currencies;
using RecruitingPlatform.Services.Locations;
using RecruitingPlatform.Services.Skills;
using RecruitingPlatform.Services.Specialties;
using RecruitingPlatform.Services.Vacancies;
using RecruitingPlatform.ViewModels.Vacancies;
using System.Security.Claims;
using Xunit;

namespace RecruitingPlatform.UnitTests.Controllers;

public class VacanciesControllerTests
{
    private readonly Mock<IGetVacanciesWithFiltersService> _getVacanciesWithFiltersServiceMock;
    private readonly Mock<IGetVacancyByIdService> _getVacancyByIdServiceMock;
    private readonly Mock<IGetAllSkillsService> _getAllSkillsServiceMock;
    private readonly Mock<ICreateVacancyService> _createVacancyServiceMock;
    private readonly Mock<IGetAllSpecialtiesService> _getAllSpecialtiesServiceMock;
    private readonly Mock<IGetAllCurrenciesService> _getAllCurrenciesServiceMock;
    private readonly Mock<IGetAllLocationsService> _getAllLocationsServiceMock;
    private readonly Mock<IEditVacancyService> _editVacancyServiceMock;
    private readonly Mock<IDeleteVacancyService> _deleteVacancyServiceMock;
    private readonly Mock<ILogger<VacanciesController>> _loggerMock;
    private readonly VacanciesController _controller;

    public VacanciesControllerTests()
    {
        _getVacanciesWithFiltersServiceMock = new Mock<IGetVacanciesWithFiltersService>();
        _getVacancyByIdServiceMock = new Mock<IGetVacancyByIdService>();
        _getAllSkillsServiceMock = new Mock<IGetAllSkillsService>();
        _createVacancyServiceMock = new Mock<ICreateVacancyService>();
        _getAllSpecialtiesServiceMock = new Mock<IGetAllSpecialtiesService>();
        _getAllCurrenciesServiceMock = new Mock<IGetAllCurrenciesService>();
        _getAllLocationsServiceMock = new Mock<IGetAllLocationsService>();
        _editVacancyServiceMock = new Mock<IEditVacancyService>();
        _deleteVacancyServiceMock = new Mock<IDeleteVacancyService>();
        _loggerMock = new Mock<ILogger<VacanciesController>>();

        _controller = new VacanciesController(
            _getVacanciesWithFiltersServiceMock.Object,
            _getVacancyByIdServiceMock.Object,
            _getAllSkillsServiceMock.Object,
            _createVacancyServiceMock.Object,
            _getAllSpecialtiesServiceMock.Object,
            _getAllCurrenciesServiceMock.Object,
            _getAllLocationsServiceMock.Object,
            _editVacancyServiceMock.Object,
            _deleteVacancyServiceMock.Object,
            _loggerMock.Object)
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
        };
    }

    private void SetUserContext(string userId, string role = "Employer")
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
        var filters = new VacancyFiltersDto { Page = 0 };

        var pagedResult = new PagedResultDto<Vacancy>
        {
            CurrentPage = 1,
            TotalPages = 1,
            TotalItems = 0,
            Items = new List<Vacancy>()
        };

        var locations = new List<Location>();
        var specialties = new List<Specialty>();

        _getVacanciesWithFiltersServiceMock.Setup(s => s.ExecuteAsync(filters)).ReturnsAsync(pagedResult);
        _getAllLocationsServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(locations);
        _getAllSpecialtiesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(specialties);

        var result = await _controller.Index(filters);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(1, filters.Page);
        Assert.Equal(locations, _controller.ViewBag.Locations);
        Assert.Equal(specialties, _controller.ViewBag.Specialties);
        Assert.Equal(filters, _controller.ViewBag.Filters);
    }

    [Fact]
    public async Task Details_Get_VacancyNotFound_ReturnsVacancyNotFoundView()
    {
        SetUserContext("1");
        _getVacancyByIdServiceMock.Setup(s => s.ExecuteAsync(1)).ReturnsAsync((Vacancy?)null);

        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("VacancyNotFound", viewResult.ViewName);
    }

    [Fact]
    public async Task Details_Get_ValidVacancy_ReturnsViewWithVacancy()
    {
        SetUserContext("1");
        var vacancy = new Vacancy { Id = 1 };
        _getVacancyByIdServiceMock.Setup(s => s.ExecuteAsync(1)).ReturnsAsync(vacancy);

        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(vacancy, viewResult.Model);
    }

    [Fact]
    public async Task Create_Get_ReturnsViewWithGroupedSkillsAndDictionaries()
    {
        SetUserContext("1");
        var specialties = new List<Specialty> { new Specialty() };
        var locations = new List<Location> { new Location() };
        var currencies = new List<Currency> { new Currency() };
        var skills = new List<Skill>
        {
            new Skill { SkillType = new SkillType { Name = "Type1" } },
            new Skill { SkillType = null }
        };

        _getAllSpecialtiesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(specialties);
        _getAllLocationsServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(locations);
        _getAllCurrenciesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(currencies);
        _getAllSkillsServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(skills);

        var result = await _controller.Create();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CreateVacancyViewModel>(viewResult.Model);

        Assert.Equal(specialties, model.Specialties);
        Assert.Equal(locations, model.Locations);
        Assert.Equal(currencies, model.Currencies);
        Assert.True(model.GroupedSkills.ContainsKey("Type1"));
        Assert.True(model.GroupedSkills.ContainsKey("Інше"));
    }

    [Fact]
    public async Task Create_Post_InvalidModelState_ReturnsViewWithModel()
    {
        SetUserContext("1");
        _controller.ModelState.AddModelError("Error", "Invalid data");
        var formData = new CreateVacancyDto();

        _getAllSpecialtiesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Specialty>());
        _getAllLocationsServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Location>());
        _getAllCurrenciesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Currency>());
        _getAllSkillsServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Skill>());

        var result = await _controller.Create(formData);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CreateVacancyViewModel>(viewResult.Model);
        Assert.Equal(formData, model.FormData);
    }

    [Fact]
    public async Task Create_Post_ValidModel_RedirectsToEmployerProfile()
    {
        SetUserContext("1");
        var formData = new CreateVacancyDto();

        var result = await _controller.Create(formData);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("EmployerProfile", redirectResult.ControllerName);
        Assert.Equal("Вакансію успішно створено!", _controller.TempData["SuccessMessage"]);
    }

    [Fact]
    public async Task Edit_Get_VacancyNotFound_ReturnsNotFound()
    {
        SetUserContext("1");
        _editVacancyServiceMock.Setup(s => s.GetForEditAsync(1, 1, false)).ReturnsAsync((EditVacancyDto?)null);

        var result = await _controller.Edit(1);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(VacanciesConstants.VacancyNotFoundErrorMessage, notFoundResult.Value);
    }

    [Fact]
    public async Task Edit_Get_ValidVacancy_ReturnsViewWithModel()
    {
        SetUserContext("1", nameof(PossibleUserRole.Admin));
        var dto = new EditVacancyDto();

        _editVacancyServiceMock.Setup(s => s.GetForEditAsync(1, 1, true)).ReturnsAsync(dto);
        _getAllSpecialtiesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Specialty>());
        _getAllLocationsServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Location>());
        _getAllCurrenciesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Currency>());
        _getAllSkillsServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Skill>());

        var result = await _controller.Edit(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EditVacancyViewModel>(viewResult.Model);
        Assert.Equal(dto, model.FormData);
    }

    [Fact]
    public async Task Edit_Post_InvalidModelState_ReturnsViewWithModel()
    {
        SetUserContext("1");
        _controller.ModelState.AddModelError("Error", "Invalid");
        var formData = new EditVacancyDto { Id = 1 };

        _getAllSpecialtiesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Specialty>());
        _getAllLocationsServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Location>());
        _getAllCurrenciesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Currency>());
        _getAllSkillsServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Skill>());

        var result = await _controller.Edit(formData);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EditVacancyViewModel>(viewResult.Model);
        Assert.Equal(formData, model.FormData);
    }

    [Fact]
    public async Task Edit_Post_UpdateFails_AddsErrorAndReturnsView()
    {
        SetUserContext("1");
        var formData = new EditVacancyDto { Id = 1 };
        _editVacancyServiceMock.Setup(s => s.UpdateAsync(1, formData, false)).ReturnsAsync(false);
        _getAllSpecialtiesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Specialty>());
        _getAllLocationsServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Location>());
        _getAllCurrenciesServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Currency>());
        _getAllSkillsServiceMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(new List<Skill>());

        var result = await _controller.Edit(formData);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Edit_Post_Success_RedirectsToDetails()
    {
        SetUserContext("1");
        var formData = new EditVacancyDto { Id = 10 };
        _editVacancyServiceMock.Setup(s => s.UpdateAsync(1, formData, false)).ReturnsAsync(true);

        var result = await _controller.Edit(formData);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectResult.ActionName);
        Assert.Equal(10, redirectResult.RouteValues["id"]);
    }

    [Fact]
    public async Task Delete_Post_Fails_RedirectsToDetails()
    {
        SetUserContext("1");
        _deleteVacancyServiceMock.Setup(s => s.DeleteAsync(1, 1, false)).ReturnsAsync(false);

        var result = await _controller.Delete(1);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectResult.ActionName);
        Assert.Equal(1, redirectResult.RouteValues["id"]);
    }

    [Fact]
    public async Task Delete_Post_SuccessAdmin_RedirectsToAdmin()
    {
        SetUserContext("1", nameof(PossibleUserRole.Admin));
        _deleteVacancyServiceMock.Setup(s => s.DeleteAsync(1, 1, true)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Admin", redirectResult.ControllerName);
        Assert.Equal("vacancies", redirectResult.RouteValues["tab"]);
    }

    [Fact]
    public async Task Delete_Post_SuccessEmployer_RedirectsToEmployerProfile()
    {
        SetUserContext("1", nameof(PossibleUserRole.Employer));
        _deleteVacancyServiceMock.Setup(s => s.DeleteAsync(1, 1, false)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("EmployerProfile", redirectResult.ControllerName);
    }
}