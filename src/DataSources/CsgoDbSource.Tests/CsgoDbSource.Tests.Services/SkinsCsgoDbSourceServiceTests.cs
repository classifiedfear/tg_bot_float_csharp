using System;
using System.Net;
using System.Net.Http.Headers;
using CsgoDbSource.Dtos.SkinsDtos;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Parsers;
using CsgoDbSource.Services;
using CsgoDbSource.Tests.ExpectedResults;
using CsgoDbSource.Tests.Fixtures;

namespace CsgoDbSource.Tests.CsgoDbSource.Tests.Services;

[Collection("csgodb")]
public class SkinsCsgoDbSourceServiceTests(
    HtmlPagesFixture htmlPages, ParserOptionsFixture parserOptions
) : BaseCsgoDbServiceTests<SkinsExpectedPage, SkinsPageDto>
{
    private static readonly SkinsExpectedPage ExpectedFamasPage = new()
    {
        WeaponName = "FAMAS",
        Skins = [
                    SkinsExpectedPage.MakeSkin("Commemoration", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Commemoration.webp", "Covert"),
                    SkinsExpectedPage.MakeSkin("Neural Net", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Neural_Net.webp", "Restricted"),
                    SkinsExpectedPage.MakeSkin("Pulse", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Pulse.webp", "Restricted"),
                ],
        SkinCount = 42
    };

    private static readonly SkinsExpectedPage ExpectedDesertEaglePage = new()
    {
        WeaponName = "Desert Eagle",
        Skins = [
                    SkinsExpectedPage.MakeSkin("Starcade", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_Starcade.webp", "Classified"),
                    SkinsExpectedPage.MakeSkin("Midnight Storm", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_Midnight_Storm.webp", "Industrial"),
                    SkinsExpectedPage.MakeSkin("Mulberry", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_Mulberry.webp", "Restricted"),
                ],
        SkinCount = 43
    };


    private static readonly SkinsExpectedPage ExpectedKarambitPage = new()
    {
        WeaponName = "Karambit",
        Skins = [
                    SkinsExpectedPage.MakeSkin("Urban Masked", "https://www.csgodatabase.com/images/knives/webp/Karambit_Urban_Masked.webp", "Extraordinary"),
                    SkinsExpectedPage.MakeSkin("Bright Water", "https://www.csgodatabase.com/images/knives/webp/Karambit_Bright_Water.webp", "Extraordinary"),
                    SkinsExpectedPage.MakeSkin("Freehand", "https://www.csgodatabase.com/images/knives/webp/Karambit_Freehand.webp", "Extraordinary"),
                ],
        SkinCount = 25
    };

    public static readonly TheoryData<SkinsExpectedPage> TestParams = new()
    {
        {ExpectedDesertEaglePage}, {ExpectedFamasPage}, {ExpectedKarambitPage}
    };

    [Theory]
    [MemberData(nameof(TestParams), DisableDiscoveryEnumeration = true)]
    public async Task GetPage_Should_Return_Parsed_Page(SkinsExpectedPage expectedPage) =>
        await ReturnParsedPage(
            expectedPage,
            new() { StatusCode = HttpStatusCode.OK, Content = new StringContent(htmlPages.GetSkinPage(expectedPage.WeaponName)) }
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

    protected override void ValidatePage(SkinsExpectedPage expected, SkinsPageDto actual)
    {
        Assert.NotEmpty(actual.Skins);
        Assert.Equal(expected.SkinCount, actual.SkinCount);
        Assert.Equal(expected.WeaponName, actual.WeaponName);

        foreach (var skin in expected.Skins)
        {
            Assert.Contains(skin, actual.Skins);
        }
    }
}

