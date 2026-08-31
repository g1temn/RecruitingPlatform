using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RecruitingPlatform.IntegrationTests.Helpers;
using Xunit;

namespace RecruitingPlatform.IntegrationTests;

public class AuthenticatedEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthenticatedEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Configure<AuthenticationOptions>(authOptions =>
                {
                    authOptions.DefaultAuthenticateScheme = "TestScheme";
                });

                services.AddAuthentication()
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
            });
        });
    }

    private HttpClient CreateClientWithRole(string role, string id = "100")
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        client.DefaultRequestHeaders.Add("Authorization", "TestScheme");
        client.DefaultRequestHeaders.Add("X-Test-Role", role);
        client.DefaultRequestHeaders.Add("X-Test-Id", id);

        return client;
    }

    [Theory]
    [InlineData("/Vacancies/Create")]
    public async Task Get_Endpoints_AsEmployer_ReturnsSuccess(string url)
    {
        var client = CreateClientWithRole("Employer");
        var response = await client.GetAsync(url);

        Assert.True(response.IsSuccessStatusCode,
            $"Expected success for {url} as Employer, but got {(int)response.StatusCode}");
    }

    [Theory]
    [InlineData("/Resumes/Create")]
    public async Task Get_Endpoints_AsJobSeeker_ReturnsSuccess(string url)
    {
        var client = CreateClientWithRole("JobSeeker");
        var response = await client.GetAsync(url);

        Assert.True(response.IsSuccessStatusCode,
            $"Expected success for {url} as JobSeeker, but got {(int)response.StatusCode}");
    }

    [Fact]
    public async Task Get_EmployerEndpoint_AsJobSeeker_ReturnsForbiddenOrRedirect()
    {
        var client = CreateClientWithRole("JobSeeker");
        var response = await client.GetAsync("/Vacancies/Create");

        Assert.True(response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Redirect,
            $"Expected Forbidden/Redirect, but got {(int)response.StatusCode}");
    }
}