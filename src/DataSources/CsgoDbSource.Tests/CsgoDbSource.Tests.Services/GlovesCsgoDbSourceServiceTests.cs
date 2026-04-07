using System.Net;
using CsgoDbSource.Dtos.GlovesDtos;
using CsgoDbSource.Parsers;
using CsgoDbSource.Services;
using CsgoDbSource.Tests.ExpectedData;
using CsgoDbSource.Tests.Fixtures;

namespace CsgoDbSource.Tests.CsgoDbSource.Tests.Services;

[Collection("csgodb")]
public class GlovesCsgoDbSourceServiceTests(
    HtmlPagesFixture htmlPages,
    ParserOptionsFixture parserOptions
) : BaseCsgoDbServiceTests<GlovesPageDto>
{
    private static readonly GlovesExpectedData expectedData = new();
    private static readonly GlovesPageDto expected = expectedData.ToPageDto();

    [Fact]
    public async Task GetPage_Should_Return_Parsed_Page() =>
        await ReturnParsedPage(
            expected,
            new() { StatusCode = HttpStatusCode.OK, Content = new StringContent(htmlPages.GlovesPage) }
        );

    [Fact]
    public async Task GetPage_Should_Throw_When_Cancellation_Requested() =>
        await CancellationRequested(new() { StatusCode = HttpStatusCode.OK, Content = new StringContent(htmlPages.GlovesPage) });

    [Fact]
    public async Task GetPage_Should_Throw_When_Source_Changed() =>
        await SourceChanged(new() { StatusCode = HttpStatusCode.OK, Content = new StringContent(htmlPages.WrongPage) });

    protected override CsgoDbSourceService<GlovesPageDto> CreateService(HttpResponseMessage responseMessage) =>
        new(
            GetHttpClientFactoryMock(responseMessage),
            new GlovesParser(parserOptions.GlovesOptions),
            GetResiliencePipeline()
        );

    protected override void ValidatePage(GlovesPageDto expected, GlovesPageDto actual)
    {
        Assert.Equal(expected.GloveCount, actual.GloveCount);
        Assert.Equal(expected.SkinCount, actual.SkinCount);
        Assert.NotEmpty(actual.Gloves);

        var actualfirstGlove = actual.Gloves.First();
        var expectedFirstGlove = expected.Gloves.First();

        Assert.Equal(expectedFirstGlove.GloveName, actualfirstGlove.GloveName);
        Assert.Equal(expectedFirstGlove.SkinCount, actualfirstGlove.SkinCount);
        Assert.NotEmpty(actualfirstGlove.Skins);
    }
}
