using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Skills;
using RecruitingPlatform.UnitTests.Helpers;
using Xunit;

namespace RecruitingPlatform.UnitTests.Services.Skills;

public class GetAllSkillsServiceTests : DbTestBase
{
    [Fact]
    public async Task ExecuteAsync_DatabaseIsEmpty_ReturnsEmptyCollection()
    {
        using var dbContext = GetInMemoryDbContext();
        var service = new GetAllSkillsService(dbContext);

        var result = await service.ExecuteAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteAsync_DatabaseHasSkills_ReturnsAllSkillsOrderedByNameWithTypes()
    {
        using var dbContext = GetInMemoryDbContext();

        var skillType1 = new SkillType { Id = 1, Name = "Type 1" };
        var skillType2 = new SkillType { Id = 2, Name = "Type 2" };

        dbContext.SkillTypes.Add(skillType1);
        dbContext.SkillTypes.Add(skillType2);

        dbContext.Skills.Add(new Skill { Id = 1, Name = "Z Skill", SkillTypeId = 1, SkillType = skillType1 });
        dbContext.Skills.Add(new Skill { Id = 2, Name = "A Skill", SkillTypeId = 2, SkillType = skillType2 });
        dbContext.Skills.Add(new Skill { Id = 3, Name = "M Skill", SkillTypeId = 1, SkillType = skillType1 });

        await dbContext.SaveChangesAsync();

        var service = new GetAllSkillsService(dbContext);

        var result = await service.ExecuteAsync();
        var resultList = result.ToList();

        Assert.NotNull(resultList);
        Assert.Equal(3, resultList.Count);
        Assert.Equal("A Skill", resultList[0].Name);
        Assert.NotNull(resultList[0].SkillType);
        Assert.Equal("M Skill", resultList[1].Name);
        Assert.Equal("Z Skill", resultList[2].Name);
    }
}