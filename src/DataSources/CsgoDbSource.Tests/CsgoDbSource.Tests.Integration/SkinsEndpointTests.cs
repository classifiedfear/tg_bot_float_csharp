using System;
using System.Net;
using System.Net.Http.Headers;
using CsgoDbSource.Dtos.SkinsDtos;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Tests.ExpectedResults;
using Microsoft.AspNetCore.Http;

namespace CsgoDbSource.Tests.CsgoDbSource.Tests.Integration;

public class SkinsEndpointTests(TestsWebApplicationFactory factory) :
    BaseEndpointTests<SkinsPageDto>(factory), IClassFixture<TestsWebApplicationFactory>
{
    private static readonly SkinsExpectedPage ExpectedFamasPageData = new()
    {
        WeaponName = "FAMAS",
        Skins = [
                    SkinsExpectedPage.MakeSkin("Rapid Eye Movement", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Rapid_Eye_Movement.webp", "Classified"),
                    SkinsExpectedPage.MakeSkin("Doomkitty", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Doomkitty.webp", "Restricted"),
                    SkinsExpectedPage.MakeSkin("Mecha Industries", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Mecha_Industries.webp", "Restricted"),
                ],
        SkinCount = 42
    };

    private static readonly SkinsExpectedPage ExpectedDesertEaglePageData = new()
    {
        WeaponName = "Desert Eagle",
        Skins = [
                    SkinsExpectedPage.MakeSkin("Sunset Storm 壱", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_Sunset_Storm_%E5%A3%B1.webp", "Restricted"),
                    SkinsExpectedPage.MakeSkin("Calligraffiti", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_Calligraffiti.webp", "Mil-spec"),
                    SkinsExpectedPage.MakeSkin("Oxide Blaze", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_Oxide_Blaze.webp", "Mil-spec"),
                ],
        SkinCount = 43
    };


    private static readonly SkinsExpectedPage ExpectedKarambitPageData = new()
    {
        WeaponName = "Karambit",
        Skins = [
                    SkinsExpectedPage.MakeSkin("Boreal Forest", "https://www.csgodatabase.com/images/knives/webp/Karambit_Boreal_Forest.webp", "Extraordinary"),
                    SkinsExpectedPage.MakeSkin("Lore", "https://www.csgodatabase.com/images/knives/webp/Karambit_Lore.webp", "Extraordinary"),
                    SkinsExpectedPage.MakeSkin("Safari Mesh", "https://www.csgodatabase.com/images/knives/webp/Karambit_Safari_Mesh.webp", "Extraordinary"),
                ],
        SkinCount = 25
    };

    public static readonly TheoryData<SkinsExpectedPage> TestData = new() {
        {ExpectedDesertEaglePageData}, {ExpectedFamasPageData}, {ExpectedKarambitPageData}
    };

    private static SkinsPageDto CreateExpectedPage(SkinsExpectedPage pageData) =>
        new() { WeaponName = pageData.WeaponName, Skins = pageData.Skins };

    protected override void SuccessValidate(SkinsPageDto expected, SkinsPageDto actual)
    {
        Assert.NotEmpty(actual.Skins);
        Assert.Equal(expected.SkinCount, actual.SkinCount);
        Assert.Equal(expected.WeaponName, actual.WeaponName);

        foreach (var skin in expected.Skins)
        {
            Assert.Contains(skin, actual.Skins);
        }
    }

    private static string BuildEndpoint(string weapon) => string.Format("weapons/{0}/skins", weapon.ToLower().Replace('-', ' '));

    [Theory]
    [MemberData(nameof(TestData), DisableDiscoveryEnumeration = true)]
    public async Task GetSkins_Should_Succeed(SkinsExpectedPage expectedData) =>
        await EndpointShouldSucceed(BuildEndpoint(expectedData.WeaponName), CreateExpectedPage(expectedData));


    [Theory]
    [MemberData(nameof(TestData), DisableDiscoveryEnumeration = true)]
    public async Task GetSkins_Should_Fail_502(SkinsExpectedPage expectedData) =>
        await EndpointShouldFail(
            BuildEndpoint(expectedData.WeaponName), new SourceStructureException(BaseCsgoDbSourceException.SourceStructureProblem),
            StatusCodes.Status502BadGateway
        );

    [Theory]
    [MemberData(nameof(TestData), DisableDiscoveryEnumeration = true)]
    public async Task GetSkins_Should_Fail_499(SkinsExpectedPage expectedData) =>
        await EndpointShouldFail(BuildEndpoint(expectedData.WeaponName), new OperationCanceledException(), StatusCodes.Status499ClientClosedRequest);

    [Theory]
    [MemberData(nameof(TestData), DisableDiscoveryEnumeration = true)]
    public async Task GetSkins_Should_Fail_500(SkinsExpectedPage expectedData) =>
        await EndpointShouldFail(BuildEndpoint(expectedData.WeaponName), new("Unexpected"), StatusCodes.Status500InternalServerError);

    [Theory]
    [InlineData("weaponName1")]
    [InlineData("weaponName2")]
    [InlineData("weaponName3")]
    public async Task GetSkins_Should_Return_404_For_Invalid_Weapon(string weaponName)
    {

        var client = Factory.CreateClient();

        var response = await client.GetAsync(BuildEndpoint(weaponName));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
