using System;
using System.Net;
using CsgoDbSource.Dtos.AdditionalInfoDtos;
using CsgoDbSource.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CsgoDbSource.Tests.CsgoDbSource.Tests.Integration;

public class AdditionalInfoEndpointTests(TestsWebApplicationFactory factory) :
    BaseEndpointTests<AdditionalInfoPageDto>(factory), IClassFixture<TestsWebApplicationFactory>
{

    private static string BuildEndpoint(string weapon, string skin) =>
        string.Format("weapons/{0}/{1}", weapon.ToLower().Replace('-', ' '), skin.ToLower().Replace('-', ' '));
    protected override void SuccessValidate(AdditionalInfoPageDto expected, AdditionalInfoPageDto actual)
    {
        Assert.Equal(expected.WeaponName, actual.WeaponName);
        Assert.Equal(expected.SkinName, actual.SkinName);
        Assert.Equal(expected.Rarity, actual.Rarity);
        Assert.Equal(expected.StattrakExistence, actual.StattrakExistence);
        Assert.Equal(expected.WeaponSkinImg, actual.WeaponSkinImg);

        Assert.All(expected.Qualities, quality => Assert.Contains(quality, actual.Qualities));
        Assert.All(expected.StattrakQualities, quality => Assert.Contains(quality, actual.StattrakQualities));
    }

    private readonly static AdditionalInfoPageDto ExpectedFiveSevenMonkeyBusinessPage = new AdditionalInfoPageDto.Builder()
            .WithWeapon("Zeus x27")
            .WithSkinName("Dragon Snore")
            .WithRarity("Classified")
            .WithQualities(["Factory New", "Minimal Wear", "Field-Tested", "Well-Worn", "Battle-Scarred"])
            .WithStattrakQualities([])
            .WithStattrakExistence(false)
            .WithImg("https://www.csgodatabase.com/images/skins/webp/Zeus_x27_Dragon_Snore.webp")
            .Build();

    private readonly static AdditionalInfoPageDto ExpectedSkeletonKnifeSlaughterPage = new AdditionalInfoPageDto.Builder()
        .WithWeapon("SCAR-20")
        .WithSkinName("Contractor")
        .WithRarity("Consumer")
        .WithQualities(["Factory New", "Minimal Wear", "Field-Tested", "Well-Worn", "Battle-Scarred"])
        .WithStattrakQualities([])
        .WithStattrakExistence(false)
        .WithImg("https://www.csgodatabase.com/images/skins/webp/SCAR-20_Contractor.webp")
        .Build();

    public static readonly TheoryData<AdditionalInfoPageDto> TestParams = new()
    {
        {ExpectedFiveSevenMonkeyBusinessPage}, {ExpectedSkeletonKnifeSlaughterPage}
    };

    [Theory]
    [MemberData(nameof(TestParams), DisableDiscoveryEnumeration = true)]
    public async Task GetWeaponSkin_Should_Succeed(AdditionalInfoPageDto expectedPage) =>
        await EndpointShouldSucceed(BuildEndpoint(expectedPage.WeaponName, expectedPage.SkinName), expectedPage);

    [Theory]
    [MemberData(nameof(TestParams), DisableDiscoveryEnumeration = true)]
    public async Task GetSkins_Should_Fail_502(AdditionalInfoPageDto expectedData) =>
        await EndpointShouldFail(
            BuildEndpoint(expectedData.WeaponName, expectedData.SkinName), new SourceStructureException(BaseCsgoDbSourceException.SourceStructureProblem),
            StatusCodes.Status502BadGateway
        );

    [Theory]
    [MemberData(nameof(TestParams), DisableDiscoveryEnumeration = true)]
    public async Task GetSkins_Should_Fail_499(AdditionalInfoPageDto expectedData) =>
        await EndpointShouldFail(BuildEndpoint(expectedData.WeaponName, expectedData.SkinName), new OperationCanceledException(), StatusCodes.Status499ClientClosedRequest);

    [Theory]
    [MemberData(nameof(TestParams), DisableDiscoveryEnumeration = true)]
    public async Task GetSkins_Should_Fail_500(AdditionalInfoPageDto expectedData) =>
        await EndpointShouldFail(BuildEndpoint(expectedData.WeaponName, expectedData.SkinName), new("Unexpected"), StatusCodes.Status500InternalServerError);

    [Theory]
    [InlineData("weaponName1", "skinName1")]
    [InlineData("weaponname2", "skinName2")]
    [InlineData("weaponName3", "skinName3")]
    public async Task GetSkins_Should_Return_404_For_Invalid_Weapon(string weaponName, string skinName)
    {

        var client = Factory.CreateClient();

        var response = await client.GetAsync(BuildEndpoint(weaponName, skinName));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
