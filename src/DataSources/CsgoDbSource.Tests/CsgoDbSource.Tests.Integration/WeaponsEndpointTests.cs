using CsgoDbSource.Dtos.WeaponsDtos;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Tests.CsgoDbSource.Tests.Integration;
using CsgoDbSource.Tests.ExpectedData;
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
    }

    private static readonly WeaponsExpectedData weaponPageData = new();
    private static readonly WeaponsPageDto expected = weaponPageData.ToPageDto();

    [Fact]
    public async Task GetWeapons_Should_Succeed() =>
        await EndpointShouldSucceed("/weapons", expected);

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
