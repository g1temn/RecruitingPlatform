using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Auth;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Auth;

public class CheckEmailServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAcync_EmailExists_ReturnsTrue()
    {
        using var dbContext = GetInMemoryDbContext();
        dbContext.Users.Add(new User { Id = 1, Email = "test@example.com" });
        await dbContext.SaveChangesAsync();

        var service = new CheckEmailExistsService(dbContext);

        var result = await service.ExecuteAcync("test@example.com");

        Assert.True(result);
    }

    [Fact]
    public async Task ExecuteAcync_EmailDoesNotExist_ReturnsFalse()
    {
        using var dbContext = GetInMemoryDbContext();
        dbContext.Users.Add(new User { Id = 1, Email = "test@example.com" });
        await dbContext.SaveChangesAsync();

        var service = new CheckEmailExistsService(dbContext);

        var result = await service.ExecuteAcync("notfound@example.com");

        Assert.False(result);
    }
}