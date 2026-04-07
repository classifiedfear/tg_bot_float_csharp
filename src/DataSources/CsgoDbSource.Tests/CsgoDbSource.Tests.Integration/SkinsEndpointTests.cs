using System.Net;
using CsgoDbSource.Dtos.SkinsDtos;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Tests.ExpectedData;
using Microsoft.AspNetCore.Http;

namespace CsgoDbSource.Tests.CsgoDbSource.Tests.Integration;

public class SkinsEndpointTests(TestsWebApplicationFactory factory) :
    BaseEndpointTests<SkinsPageDto>(factory), IClassFixture<TestsWebApplicationFactory>
{
    private static readonly SkinsExpectedData ExpectedFamasPageData = new()
    {
        WeaponName = "FAMAS",
        Skins = [
                    SkinsExpectedData.MakeSkin("Rapid Eye Movement", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Rapid_Eye_Movement.webp", "Classified"),
                    SkinsExpectedData.MakeSkin("Doomkitty", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Doomkitty.webp", "Restricted"),
                    SkinsExpectedData.MakeSkin("Mecha Industries", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Mecha_Industries.webp", "Restricted"),
                ],
        SkinCount = 42
    };

    private static readonly SkinsExpectedData ExpectedDesertEaglePageData = new()
    {
        WeaponName = "Desert Eagle",
        Skins = [
                    SkinsExpectedData.MakeSkin("Sunset Storm 壱", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_Sunset_Storm_%E5%A3%B1.webp", "Restricted"),
                    SkinsExpectedData.MakeSkin("Calligraffiti", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_Calligraffiti.webp", "Mil-spec"),
                    SkinsExpectedData.MakeSkin("Oxide Blaze", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_Oxide_Blaze.webp", "Mil-spec"),
                ],
        SkinCount = 43
    };


    private static readonly SkinsExpectedData ExpectedKarambitPageData = new()
    {
        WeaponName = "Karambit",
        Skins = [
                    SkinsExpectedData.MakeSkin("Boreal Forest", "https://www.csgodatabase.com/images/knives/webp/Karambit_Boreal_Forest.webp", "Extraordinary"),
                    SkinsExpectedData.MakeSkin("Lore", "https://www.csgodatabase.com/images/knives/webp/Karambit_Lore.webp", "Extraordinary"),
                    SkinsExpectedData.MakeSkin("Safari Mesh", "https://www.csgodatabase.com/images/knives/webp/Karambit_Safari_Mesh.webp", "Extraordinary"),
                ],
        SkinCount = 24
    };

    public static readonly TheoryData<SkinsPageDto> TestData = new() {
        { ExpectedDesertEaglePageData.ToPageDto() },
        { ExpectedFamasPageData.ToPageDto() },
        { ExpectedKarambitPageData.ToPageDto() }
    };

    protected override void SuccessValidate(SkinsPageDto expected, SkinsPageDto actual)
    {
        Assert.NotEmpty(actual.Skins);
        Assert.Equal(expected.SkinCount, actual.SkinCount);
        Assert.Equal(expected.WeaponName, actual.WeaponName);
    }

    private static string BuildEndpoint(string weapon) => string.Format("weapons/{0}/skins", weapon.ToLower().Replace('-', ' '));

    [Theory]
    [MemberData(nameof(TestData), DisableDiscoveryEnumeration = true)]
    public async Task GetSkins_Should_Succeed(SkinsPageDto expected) =>
        await EndpointShouldSucceed(BuildEndpoint(expected.WeaponName), expected);


    [Theory]
    [MemberData(nameof(TestData), DisableDiscoveryEnumeration = true)]
    public async Task GetSkins_Should_Fail_502(SkinsPageDto expected) =>
        await EndpointShouldFail(
            BuildEndpoint(expected.WeaponName), new SourceStructureException(BaseCsgoDbSourceException.SourceStructureProblem),
            StatusCodes.Status502BadGateway
        );

    [Theory]
    [MemberData(nameof(TestData), DisableDiscoveryEnumeration = true)]
    public async Task GetSkins_Should_Fail_499(SkinsPageDto expected) =>
        await EndpointShouldFail(BuildEndpoint(expected.WeaponName), new OperationCanceledException(), StatusCodes.Status499ClientClosedRequest);

    [Theory]
    [MemberData(nameof(TestData), DisableDiscoveryEnumeration = true)]
    public async Task GetSkins_Should_Fail_500(SkinsPageDto expected) =>
        await EndpointShouldFail(BuildEndpoint(expected.WeaponName), new("Unexpected"), StatusCodes.Status500InternalServerError);

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
