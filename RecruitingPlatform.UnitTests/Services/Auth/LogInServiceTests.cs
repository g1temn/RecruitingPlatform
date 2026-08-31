using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using RecruitingPlatform.DTOs.Auth;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Auth;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Auth;

public class LogInServiceTests
{
    private Mock<SignInManager<User>> GetMockSignInManager()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();

        return new Mock<SignInManager<User>>(
            userManagerMock.Object,
            contextAccessorMock.Object,
            claimsFactoryMock.Object,
            null, null, null, null);
    }

    [Fact]
    public async Task ExecuteAsync_ValidCredentials_ReturnsTrue()
    {
        var signInManagerMock = GetMockSignInManager();
        signInManagerMock.Setup(s => s.PasswordSignInAsync("test", "Password123", true, false))
            .ReturnsAsync(SignInResult.Success);

        var service = new LogInService(signInManagerMock.Object);
        var dto = new LogInDto { Email = "test@example.com", Password = "Password123" };

        var result = await service.ExecuteAsync(dto);

        Assert.True(result);
    }

    [Fact]
    public async Task ExecuteAsync_InvalidCredentials_ReturnsFalse()
    {
        var signInManagerMock = GetMockSignInManager();
        signInManagerMock.Setup(s => s.PasswordSignInAsync("test", "WrongPassword", true, false))
            .ReturnsAsync(SignInResult.Failed);

        var service = new LogInService(signInManagerMock.Object);
        var dto = new LogInDto { Email = "test@example.com", Password = "WrongPassword" };

        var result = await service.ExecuteAsync(dto);

        Assert.False(result);
    }
}