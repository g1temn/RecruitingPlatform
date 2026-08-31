using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RecruitingPlatform.IntegrationTests.Helpers;
using Xunit;

namespace RecruitingPlatform.IntegrationTests;

public class AdminFlowIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AdminFlowIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
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
        }).CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        _client.DefaultRequestHeaders.Add("Authorization", "TestScheme");
        _client.DefaultRequestHeaders.Add("X-Test-Role", "Admin");
        _client.DefaultRequestHeaders.Add("X-Test-Id", "999");
    }

    [Fact]
    public async Task Get_AdminPanel_AsAdmin_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/Admin");

        Assert.True(response.IsSuccessStatusCode,
            $"Expected success for Admin Panel, but got {(int)response.StatusCode}");

        var htmlContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("<!DOCTYPE html>", htmlContent, StringComparison.OrdinalIgnoreCase);
    }
}