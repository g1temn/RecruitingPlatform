using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Auth;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Auth;

public class LogOutServiceTests
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
    public async Task ExecuteAsync_CallsSignOutAsync()
    {
        var signInManagerMock = GetMockSignInManager();
        signInManagerMock.Setup(s => s.SignOutAsync()).Returns(Task.CompletedTask);

        var service = new LogOutService(signInManagerMock.Object);

        await service.ExecuteAsync();

        signInManagerMock.Verify(s => s.SignOutAsync(), Times.Once);
    }
}