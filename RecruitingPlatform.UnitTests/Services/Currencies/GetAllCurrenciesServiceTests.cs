using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Currencies;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Currencies;

public class GetAllCurrenciesServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAsync_DatabaseIsEmpty_ReturnsEmptyCollection()
    {
        using var dbContext = GetInMemoryDbContext();
        var service = new GetAllCurrenciesService(dbContext);

        var result = await service.ExecuteAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteAsync_DatabaseHasCurrencies_ReturnsAllCurrencies()
    {
        using var dbContext = GetInMemoryDbContext();

        dbContext.Currencies.AddRange(
            new Currency { Id = 1, Name = "USD" },
            new Currency { Id = 2, Name = "EUR" },
            new Currency { Id = 3, Name = "UAH" }
        );
        await dbContext.SaveChangesAsync();

        var service = new GetAllCurrenciesService(dbContext);

        var result = await service.ExecuteAsync();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, c => c.Name == "EUR");
    }
}