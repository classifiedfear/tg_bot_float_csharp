using CsgoDbSource.Dtos.WeaponsDtos;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Tests.CsgoDbSource.Tests.Integration;
using CsgoDbSource.Tests.ExpectedResults;
using Microsoft.AspNetCore.Http;

namespace CsgoDbSource.Tests;

public class WeaponsEndpointTests(TestsWebApplicationFactory factory) :
    BaseEndpointTests<WeaponsPageDto>(factory), IClassFixture<TestsWebApplicationFactory>
{
    protected override void SuccessValidate(WeaponsPageDto expected, WeaponsPageDto actual)
    {
        Assert.Equal(expected.CategoryCount, actual.CategoryCount);
        Assert.Equal(expected.WeaponCount, actual.WeaponCount);
        Assert.NotEmpty(actual.Categories);

        var firstCategory = actual.Categories.First();
        var expectedFirstCategory = expected.Categories.First();
        Assert.Equal(expectedFirstCategory.Category, firstCategory.Category);
        Assert.Equal(expectedFirstCategory.WeaponInCategoryCount, firstCategory.WeaponInCategoryCount);
        Assert.NotEmpty(firstCategory.Weapons);
    }

    private static readonly WeaponsExpectedPage weaponPageData = new();

    private static WeaponsPageDto CreateExpectedPage(WeaponsExpectedPage expectedData)
    {
        var categories = new List<CategoryWeaponDto>();

        for (int i = 0; i < expectedData.CategoryCount; i++)
        {
            categories.Add(
                new CategoryWeaponDto()
                {
                    Category = expectedData.CategoryNames[i],
                    WeaponInCategoryCount = expectedData.WeaponCountEachCategory[i],
                    Weapons = [expectedData.OneWeaponEachCategory[i]]
                });
        }

        return new WeaponsPageDto()
        {
            WeaponCount = expectedData.WeaponCount,
            Categories = categories
        };
    }

    [Fact]
    public async Task GetWeapons_Should_Succeed() =>
        await EndpointShouldSucceed("/weapons", CreateExpectedPage(weaponPageData));

    [Fact]
    public async Task GetWeapons_Should_Fail_502() =>
        await EndpointShouldFail(
            "/weapons",
            new SourceStructureException(BaseCsgoDbSourceException.SourceStructureProblem),
            StatusCodes.Status502BadGateway
        );

    [Fact]
    public async Task GetWeapons_Should_Fail_499() =>
        await EndpointShouldFail("/weapons", new OperationCanceledException(), StatusCodes.Status499ClientClosedRequest);

    [Fact]
    public async Task GetWeapons_Should_Fail_500() =>
        await EndpointShouldFail("/weapons", new("Unexpected"), StatusCodes.Status500InternalServerError);
}
