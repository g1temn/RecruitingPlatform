using Microsoft.AspNetCore.Mvc.Testing;
using RecruitingPlatform.IntegrationTests.Helpers;
using System.Net;
using Xunit;

namespace RecruitingPlatform.IntegrationTests;

public class SecurityRedirectTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SecurityRedirectTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Theory]
    [InlineData("/Vacancies")]
    [InlineData("/Resumes")]
    [InlineData("/Vacancies/Create")]
    [InlineData("/Resumes/Create")]
    [InlineData("/Profile")]
    [InlineData("/EmployerProfile")]
    public async Task Get_ProtectedEndpoints_Unauthenticated_ReturnsRedirectToLogin(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        Assert.Contains("/Auth/LogIn", response.Headers.Location?.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Auth/LogIn")]
    [InlineData("/Auth/SignUp")]
    public async Task Get_PublicEndpoints_ReturnsSuccess(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.True(response.IsSuccessStatusCode,
            $"Expected success status code for {url}, but got {(int)response.StatusCode} ({response.StatusCode})");

        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }
}