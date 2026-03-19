using System;
using System.Net;
using CsgoDbSource.Dtos.GlovesDtos;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Parsers;
using CsgoDbSource.Services;
using CsgoDbSource.Tests.ExpectedResults;
using CsgoDbSource.Tests.Fixtures;

namespace CsgoDbSource.Tests.CsgoDbSource.Tests.Services;

[Collection("csgodb")]
public class GlovesCsgoDbSourceServiceTests(
    HtmlPagesFixture htmlPages,
    ParserOptionsFixture parserOptions
) : BaseCsgoDbServiceTests<GlovesExpectedPage, GlovesPageDto>
{
    private static readonly GlovesExpectedPage expectedPage = new();

    [Fact]
    public async Task GetPage_Should_Return_Parsed_Page() =>
        await ReturnParsedPage(
            expectedPage,
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

    protected override void ValidatePage(GlovesExpectedPage expected, GlovesPageDto actual)
    {
        Assert.Equal(expected.GloveCount, actual.GlovesCount);
        Assert.Equal(expected.SkinCount, actual.SkinsCount);
        Assert.NotEmpty(actual.Gloves);

        var firstGlove = actual.Gloves.First();
        Assert.Equal(expected.GloveNames.First(), firstGlove.GloveName);
        Assert.NotEmpty(firstGlove.Skins);
        Assert.Equal(expected.SkinCountEachGlove.First(), firstGlove.SkinCount);
    }
}
