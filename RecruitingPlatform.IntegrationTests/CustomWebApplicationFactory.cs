using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecruitingPlatform.Data;

namespace RecruitingPlatform.IntegrationTests.Helpers;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly string _dbName = $"IntegrationTestsDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            var optionsDescriptors = services.Where(d => d.ServiceType.Name.Contains("DbContextOptions")).ToList();
            foreach (var descriptor in optionsDescriptors) services.Remove(descriptor);

            var connectionDescriptors = services.Where(d => d.ServiceType.Name.Contains("DbConnection")).ToList();
            foreach (var descriptor in connectionDescriptors) services.Remove(descriptor);

            services.AddDbContext<RecruitingPlatformDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });
        });
    }
}