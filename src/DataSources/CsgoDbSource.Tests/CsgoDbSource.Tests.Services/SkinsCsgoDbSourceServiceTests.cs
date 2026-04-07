using System;
using System.Net;
using System.Net.Http.Headers;
using CsgoDbSource.Dtos.SkinsDtos;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Parsers;
using CsgoDbSource.Services;
using CsgoDbSource.Tests.ExpectedData;
using CsgoDbSource.Tests.Fixtures;

namespace CsgoDbSource.Tests.CsgoDbSource.Tests.Services;

[Collection("csgodb")]
public class SkinsCsgoDbSourceServiceTests(
    HtmlPagesFixture htmlPages, ParserOptionsFixture parserOptions
) : BaseCsgoDbServiceTests<SkinsPageDto>
{
    private static readonly SkinsExpectedData ExpectedFamasPage = new()
    {
        WeaponName = "FAMAS",
        Skins = [
                    SkinsExpectedData.MakeSkin("Commemoration", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Commemoration.webp", "Covert"),
                    SkinsExpectedData.MakeSkin("Neural Net", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Neural_Net.webp", "Restricted"),
                    SkinsExpectedData.MakeSkin("Pulse", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Pulse.webp", "Restricted"),
                ],
        SkinCount = 42
    };

    private static readonly SkinsExpectedData ExpectedDesertEaglePage = new()
    {
        WeaponName = "Desert Eagle",
        Skins = [
                    SkinsExpectedData.MakeSkin("Starcade", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_Starcade.webp", "Classified"),
                    SkinsExpectedData.MakeSkin("Midnight Storm", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_Midnight_Storm.webp", "Industrial"),
                    SkinsExpectedData.MakeSkin("Mulberry", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_Mulberry.webp", "Restricted"),
                ],
        SkinCount = 43
    };


    private static readonly SkinsExpectedData ExpectedKarambitPage = new()
    {
        WeaponName = "Karambit",
        Skins = [
                    SkinsExpectedData.MakeSkin("Urban Masked", "https://www.csgodatabase.com/images/knives/webp/Karambit_Urban_Masked.webp", "Extraordinary"),
                    SkinsExpectedData.MakeSkin("Bright Water", "https://www.csgodatabase.com/images/knives/webp/Karambit_Bright_Water.webp", "Extraordinary"),
                    SkinsExpectedData.MakeSkin("Freehand", "https://www.csgodatabase.com/images/knives/webp/Karambit_Freehand.webp", "Extraordinary"),
                ],
        SkinCount = 24
    };

    public static readonly TheoryData<SkinsPageDto> TestParams = new()
    {
        { ExpectedDesertEaglePage.ToPageDto() },
        { ExpectedFamasPage.ToPageDto() },
        { ExpectedKarambitPage.ToPageDto() }
    };

    [Theory]
    [MemberData(nameof(TestParams), DisableDiscoveryEnumeration = true)]
    public async Task GetPage_Should_Return_Parsed_Page(SkinsPageDto expected) =>
        await ReturnParsedPage(
            expected,
            new() { StatusCode = HttpStatusCode.OK, Content = new StringContent(htmlPages.GetSkinPage(expected.WeaponName)) }
        );

    [Fact]
    public async Task GetPage_Should_Throw_When_Cancellation_Requested() =>
        await CancellationRequested(
            new() { StatusCode = HttpStatusCode.OK, Content = new StringContent(htmlPages.WrongPage) }
        );

    [Fact]
    public async Task GetPage_Should_Throw_When_Source_Changed() =>
        await SourceChanged(
            new() { StatusCode = HttpStatusCode.OK, Content = new StringContent(htmlPages.WrongPage) }
        );


    [Fact]
    public async Task GetPage_Should_Throw_When_Wrong_Args()
    {
        var service = CreateService(new() { StatusCode = HttpStatusCode.OK, Content = new StringContent("") });

        await Assert.ThrowsAsync<PageException>(() => service.GetPage("https://test", CancellationToken.None));
    }

    protected override CsgoDbSourceService<SkinsPageDto> CreateService(HttpResponseMessage responseMessage) =>
        new(GetHttpClientFactoryMock(responseMessage),
        new SkinsParser(parserOptions.SkinsOptions),
        GetResiliencePipeline());

    protected override void ValidatePage(SkinsPageDto expected, SkinsPageDto actual)
    {
        Assert.NotEmpty(actual.Skins);
        Assert.Equal(expected.SkinCount, actual.SkinCount);
        Assert.Equal(expected.WeaponName, actual.WeaponName);
    }
}

