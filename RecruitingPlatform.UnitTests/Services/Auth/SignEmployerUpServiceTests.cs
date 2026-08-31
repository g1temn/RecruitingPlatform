using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using RecruitingPlatform.DTOs.Auth;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Enums;
using RecruitingPlatform.Services.Auth;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Auth;

public class SignEmployerUpServiceTests : DbTestBase
{
    private Mock<UserManager<User>> GetMockUserManager()
    {
        var storeMock = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(storeMock.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task ExecuteAsync_UserCreationFails_ReturnsFalse()
    {
        using var dbContext = GetInMemoryDbContext();
        var userManagerMock = GetMockUserManager();
        userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());

        var service = new SignEmployerUpService(userManagerMock.Object, dbContext);
        var dto = new SignEmployerUpDto { Email = "test@example.com", Password = "Password123", Role = PossibleUserRole.Employer };

        var result = await service.ExecuteAsync(dto);

        Assert.False(result);
        Assert.Equal(0, await dbContext.Companies.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_UserCreationSucceeds_AddsCompanyAndReturnsTrue()
    {
        using var dbContext = GetInMemoryDbContext();
        var userManagerMock = GetMockUserManager();

        userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .Callback<User, string>((u, p) => u.Id = 100)
            .ReturnsAsync(IdentityResult.Success);

        userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var service = new SignEmployerUpService(userManagerMock.Object, dbContext);
        var dto = new SignEmployerUpDto
        {
            Email = "company@example.com",
            Password = "Password123",
            Role = PossibleUserRole.Employer,
            Name = "Tech Corp",
            WebsiteUrl = "https://techcorp.com",
            ContactPhone = "123456789",
            Description = "A tech company"
        };

        var result = await service.ExecuteAsync(dto);

        Assert.True(result);

        var savedCompany = await dbContext.Companies.FirstOrDefaultAsync();
        Assert.NotNull(savedCompany);
        Assert.Equal(100, savedCompany.Id);
        Assert.Equal("Tech Corp", savedCompany.Name);
        Assert.Equal("https://techcorp.com", savedCompany.WebsiteUrl);
        Assert.Equal("123456789", savedCompany.ContactPhone);
        Assert.Equal("A tech company", savedCompany.Description);
        Assert.False(savedCompany.IsDeleted);
    }
}