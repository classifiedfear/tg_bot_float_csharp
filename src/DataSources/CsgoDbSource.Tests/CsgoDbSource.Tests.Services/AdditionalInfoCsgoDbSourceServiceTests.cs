using System;
using System.Net;
using CsgoDbSource.Dtos.AdditionalInfoDtos;
using CsgoDbSource.Parsers;
using CsgoDbSource.Services;
using CsgoDbSource.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CsgoDbSource.Tests.CsgoDbSource.Tests.Services;

[Collection("csgodb")]
public class AdditionalInfoCsgoDbSourceServiceTests(
    HtmlPagesFixture htmlPages, ParserOptionsFixture parserOptions
) : BaseCsgoDbServiceTests<AdditionalInfoPageDto>
{
    protected override CsgoDbSourceService<AdditionalInfoPageDto> CreateService(HttpResponseMessage responseMessage) =>
        new(GetHttpClientFactoryMock(responseMessage),
        new AdditionalInfoParser(parserOptions.AdditionalInfoOptions),
        GetResiliencePipeline());

    protected override void ValidatePage(AdditionalInfoPageDto expected, AdditionalInfoPageDto actual)
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
            .WithWeapon("Five-SeveN")
            .WithSkinName("Monkey Business")
            .WithRarity("Classified")
            .WithQualities(["Minimal Wear", "Field-Tested", "Well-Worn", "Battle-Scarred"])
            .WithStattrakQualities(["Minimal Wear", "Field-Tested", "Well-Worn", "Battle-Scarred"])
            .WithStattrakExistence(true)
            .WithImg("https://www.csgodatabase.com/images/skins/webp/Five-SeveN_Monkey_Business.webp")
            .Build();

    private readonly static AdditionalInfoPageDto ExpectedSkeletonKnifeSlaughterPage = new AdditionalInfoPageDto.Builder()
        .WithWeapon("Skeleton Knife")
        .WithSkinName("Slaughter")
        .WithRarity("Extraordinary")
        .WithQualities(["Factory New", "Minimal Wear", "Field-Tested"])
        .WithStattrakQualities(["Factory New", "Minimal Wear", "Field-Tested"])
        .WithStattrakExistence(true)
        .WithImg("https://www.csgodatabase.com/images/knives/webp/Skeleton_Knife_Slaughter.webp")
        .Build();

    public static readonly TheoryData<AdditionalInfoPageDto> TestParams = new()
    {
        {ExpectedFiveSevenMonkeyBusinessPage}, {ExpectedSkeletonKnifeSlaughterPage}
    };

    [Theory]
    [MemberData(nameof(TestParams), DisableDiscoveryEnumeration = true)]
    public async Task GetPage_Should_Return_Parsed_Page(AdditionalInfoPageDto expected) =>
        await ReturnParsedPage(expected, new() { StatusCode = HttpStatusCode.OK, Content = new StringContent(htmlPages.GetAdditionalInfoPage(expected.WeaponName, expected.SkinName)) });

    [Fact]
    public async Task GetPage_Should_Throw_When_Cancellation_Requested() =>
        await CancellationRequested(new() { StatusCode = HttpStatusCode.OK, Content = new StringContent(htmlPages.WrongPage) });

    [Fact]
    public async Task GetPage_Should_Throw_When_Source_Changed() =>
        await SourceChanged(new() { StatusCode = HttpStatusCode.OK, Content = new StringContent(htmlPages.WrongPage) });
}
