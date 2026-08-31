using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RecruitingPlatform.Data;
using RecruitingPlatform.Entities;
using RecruitingPlatform.IntegrationTests.Helpers;
using Xunit;

namespace RecruitingPlatform.IntegrationTests;

public class JobSeekerFlowIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _clonedFactory;

    public JobSeekerFlowIntegrationTests(CustomWebApplicationFactory<Program> factory)
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
        _client.DefaultRequestHeaders.Add("X-Test-Role", "JobSeeker");
        _client.DefaultRequestHeaders.Add("X-Test-Id", "200");
    }

    [Fact]
    public async Task Get_JobSeekerProfile_DisplaysSeekerDataFromDatabase()
    {
        using (var scope = _clonedFactory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<RecruitingPlatformDbContext>();

            dbContext.Users.RemoveRange(dbContext.Users);
            dbContext.JobSeekers.RemoveRange(dbContext.JobSeekers);

            dbContext.Users.Add(new User
            {
                Id = 200,
                UserName = "taras@test.com",
                Email = "taras@test.com",
                PasswordHash = "dummy_hash",
                IsDeleted = false
            });

            dbContext.JobSeekers.Add(new JobSeeker
            {
                Id = 200,
                FirstName = "Taras",
                LastName = "Shevchenko",
                ContactPhone = "0991234567",
                ContactEmail = "taras@test.com",
                Birthday = new DateOnly(1990, 1, 1),
                IsDeleted = false
            });
            await dbContext.SaveChangesAsync();
        }

        var response = await _client.GetAsync("/JobSeekerProfile");

        Assert.True(response.IsSuccessStatusCode,
            $"Expected success, but got {(int)response.StatusCode}. URL: {response.RequestMessage?.RequestUri}");

        var htmlContent = await response.Content.ReadAsStringAsync();

        Assert.Contains("Taras", htmlContent);
        Assert.Contains("Shevchenko", htmlContent);
    }
}