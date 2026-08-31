using Microsoft.AspNetCore.Identity;
using Moq;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Enums;
using RecruitingPlatform.Services.Auth;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Auth;

public class SignUpBaseServiceTests
{
    private class TestSignUpService : SignUpBaseService
    {
        public TestSignUpService(UserManager<User> userManager) : base(userManager) { }

        public Task<User?> ExecuteCreateUserWithRoleAsync(string email, string password, PossibleUserRole role)
        {
            return CreateUserWithRoleAsync(email, password, role);
        }
    }

    private Mock<UserManager<User>> GetMockUserManager()
    {
        var storeMock = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(storeMock.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task CreateUserWithRoleAsync_SuccessfulCreation_ReturnsUserAndAddsRole()
    {
        var userManagerMock = GetMockUserManager();
        userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), "Password123"))
            .ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), PossibleUserRole.JobSeeker.ToString()))
            .ReturnsAsync(IdentityResult.Success);

        var service = new TestSignUpService(userManagerMock.Object);

        var result = await service.ExecuteCreateUserWithRoleAsync("test@example.com", "Password123", PossibleUserRole.JobSeeker);

        Assert.NotNull(result);
        Assert.Equal("test", result.UserName);
        Assert.Equal("test@example.com", result.Email);
        userManagerMock.Verify(x => x.CreateAsync(It.IsAny<User>(), "Password123"), Times.Once);
        userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), PossibleUserRole.JobSeeker.ToString()), Times.Once);
    }

    [Fact]
    public async Task CreateUserWithRoleAsync_CreationFails_ReturnsNull()
    {
        var userManagerMock = GetMockUserManager();
        userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), "Password123"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

        var service = new TestSignUpService(userManagerMock.Object);

        var result = await service.ExecuteCreateUserWithRoleAsync("test@example.com", "Password123", PossibleUserRole.JobSeeker);

        Assert.Null(result);
        userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }
}