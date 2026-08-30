using Microsoft.AspNetCore.Mvc;
using Moq;
using RecruitingPlatform.Controllers;
using RecruitingPlatform.DTOs.Resumes;
using RecruitingPlatform.DTOs.Vacancies;
using RecruitingPlatform.Services.Resumes;
using RecruitingPlatform.Services.Vacancies;
using RecruitingPlatform.ViewModels.Admin;
using Xunit;

namespace RecruitingPlatform.UnitTests.Controllers;

public class AdminControllerTests
{
    private readonly Mock<IGetResumesWithFiltersService> _getResumesServiceMock;
    private readonly Mock<IGetVacanciesWithFiltersService> _getVacanciesServiceMock;
    private readonly AdminController _controller;

    public AdminControllerTests()
    {
        _getResumesServiceMock = new Mock<IGetResumesWithFiltersService>();
        _getVacanciesServiceMock = new Mock<IGetVacanciesWithFiltersService>();

        _controller = new AdminController(
            _getResumesServiceMock.Object,
            _getVacanciesServiceMock.Object);
    }

    [Fact]
    public async Task Index_Get_TabResumes_FetchesResumesAndReturnsView()
    {
        int page = 1;
        string tab = "resumes";

        var result = await _controller.Index(tab, page);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AdminDashboardViewModel>(viewResult.Model);

        Assert.Equal(tab, model.ActiveTab);
        Assert.Equal(page, model.CurrentPage);

        _getResumesServiceMock.Verify(s => s.ExecuteAsync(It.Is<ResumeFiltersDto>(f => f.Page == page)), Times.Once);
        _getVacanciesServiceMock.Verify(s => s.ExecuteAsync(It.IsAny<VacancyFiltersDto>()), Times.Never);
    }

    [Fact]
    public async Task Index_Get_TabVacancies_FetchesVacanciesAndReturnsView()
    {
        int page = 2;
        string tab = "vacancies";

        var result = await _controller.Index(tab, page);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AdminDashboardViewModel>(viewResult.Model);

        Assert.Equal(tab, model.ActiveTab);
        Assert.Equal(page, model.CurrentPage);

        _getVacanciesServiceMock.Verify(s => s.ExecuteAsync(It.Is<VacancyFiltersDto>(f => f.Page == page)), Times.Once);
        _getResumesServiceMock.Verify(s => s.ExecuteAsync(It.IsAny<ResumeFiltersDto>()), Times.Never);
    }

    [Fact]
    public async Task Index_Get_UnknownTab_DoesNotFetchDataAndReturnsView()
    {
        int page = 1;
        string tab = "unknown_tab";

        var result = await _controller.Index(tab, page);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AdminDashboardViewModel>(viewResult.Model);

        Assert.Equal(tab, model.ActiveTab);
        Assert.Equal(page, model.CurrentPage);

        _getResumesServiceMock.Verify(s => s.ExecuteAsync(It.IsAny<ResumeFiltersDto>()), Times.Never);
        _getVacanciesServiceMock.Verify(s => s.ExecuteAsync(It.IsAny<VacancyFiltersDto>()), Times.Never);
    }
}