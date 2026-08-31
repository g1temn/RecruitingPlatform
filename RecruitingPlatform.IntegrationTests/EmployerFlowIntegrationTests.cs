using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RecruitingPlatform.Data;
using RecruitingPlatform.Entities;
using RecruitingPlatform.IntegrationTests.Helpers;
using Xunit;

namespace RecruitingPlatform.IntegrationTests;

public class EmployerFlowIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _clonedFactory;

    public EmployerFlowIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _clonedFactory = factory.WithWebHostBuilder(builder =>
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

        _client = _clonedFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        _client.DefaultRequestHeaders.Add("Authorization", "TestScheme");
        _client.DefaultRequestHeaders.Add("X-Test-Role", "Employer");
        _client.DefaultRequestHeaders.Add("X-Test-Id", "100");
    }

    [Fact]
    public async Task Get_EmployerProfile_DisplaysCompanyDataFromDatabase()
    {
        using (var scope = _clonedFactory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<RecruitingPlatformDbContext>();

            dbContext.Users.RemoveRange(dbContext.Users);
            dbContext.Companies.RemoveRange(dbContext.Companies);

            dbContext.Users.Add(new User
            {
                Id = 100,
                UserName = "google@test.com",
                Email = "google@test.com",
                PasswordHash = "dummy_hash",
                IsDeleted = false
            });

            dbContext.Companies.Add(new Company
            {
                Id = 100,
                Name = "Google Integration Test Corp",
                Description = "Integration Test Description",
                ContactPhone = "123456789",
                WebsiteUrl = "https://google.com",
                IsDeleted = false
            });

            await dbContext.SaveChangesAsync();
        }

        var response = await _client.GetAsync("/EmployerProfile");

        Assert.True(response.IsSuccessStatusCode,
            $"Expected success, but got {(int)response.StatusCode}. URL: {response.RequestMessage?.RequestUri}");

        var htmlContent = await response.Content.ReadAsStringAsync();

        Assert.Contains("Google Integration Test Corp", htmlContent);
        Assert.Contains("Integration Test Description", htmlContent);
    }
}