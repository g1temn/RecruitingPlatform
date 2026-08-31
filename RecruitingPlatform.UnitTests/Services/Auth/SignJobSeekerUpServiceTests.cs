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

public class SignJobSeekerUpServiceTests : DbTestBase
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

        var service = new SignJobSeekerUpService(userManagerMock.Object, dbContext);
        var dto = new SignJobSeekerUpDto { Email = "test@example.com", Password = "Password123" };

        var result = await service.ExecuteAsync(dto);

        Assert.False(result);
        Assert.Equal(0, await dbContext.JobSeekers.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_UserCreationSucceeds_AddsJobSeekerAndReturnsTrue()
    {
        using var dbContext = GetInMemoryDbContext();
        var userManagerMock = GetMockUserManager();

        userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .Callback<User, string>((u, p) => u.Id = 200)
            .ReturnsAsync(IdentityResult.Success);

        userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var service = new SignJobSeekerUpService(userManagerMock.Object, dbContext);

        var birthday = new DateOnly(1990, 1, 1);
        var dto = new SignJobSeekerUpDto
        {
            Email = "seeker@example.com",
            Password = "Password123",
            FirstName = "John",
            LastName = "Doe",
            ContactPhone = "987654321",
            ContactEmail = "contact@example.com",
            Birthday = birthday
        };

        var result = await service.ExecuteAsync(dto);

        Assert.True(result);

        var savedSeeker = await dbContext.JobSeekers.FirstOrDefaultAsync();
        Assert.NotNull(savedSeeker);
        Assert.Equal(200, savedSeeker.Id);
        Assert.Equal("John", savedSeeker.FirstName);
        Assert.Equal("Doe", savedSeeker.LastName);
        Assert.Equal("987654321", savedSeeker.ContactPhone);
        Assert.Equal("contact@example.com", savedSeeker.ContactEmail);
        Assert.Equal(birthday, savedSeeker.Birthday);
        Assert.False(savedSeeker.IsDeleted);
    }
}