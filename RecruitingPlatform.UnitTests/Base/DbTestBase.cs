using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;

namespace RecruitingPlatform.UnitTests.Helpers;

public abstract class DbTestBase
{
    protected RecruitingPlatformDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<RecruitingPlatformDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new RecruitingPlatformDbContext(options);
    }
}