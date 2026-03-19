using CsgoDbSource.Dtos.WeaponsDtos;
using CsgoDbSource.Parsers;
using CsgoDbSource.Services;
using Microsoft.VisualBasic;
using CsgoDbSource.Tests.Fixtures;
using CsgoDbSource.Tests.ExpectedResults;
using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace CsgoDbSource.Tests;

[Collection("csgodb")]
public class WeaponsCsgoDbSourceServiceTests(
    HtmlPagesFixture htmlPages, ParserOptionsFixture parserOptions
) : BaseCsgoDbServiceTests<WeaponsExpectedPage, WeaponsPageDto>
{
    private static readonly WeaponsExpectedPage expectedPage = new();

    [Fact]
    public async Task GetPage_Should_Return_Parsed_Page() =>
        await ReturnParsedPage(
            expectedPage, new() { StatusCode = HttpStatusCode.OK, Content = new StringContent(htmlPages.WeaponsPage) }
        );

    [Fact]
    public async Task GetPage_Should_Throw_When_Cancellation_Requested() =>
        await CancellationRequested(
            new() { StatusCode = HttpStatusCode.OK, Content = new StringContent(htmlPages.WeaponsPage) }
        );

    [Fact]
    public async Task GetPage_Should_Throw_When_Source_Changed() =>
        await SourceChanged(
            new() { StatusCode = HttpStatusCode.OK, Content = new StringContent(htmlPages.WrongPage) }
        );

    protected override CsgoDbSourceService<WeaponsPageDto> CreateService(HttpResponseMessage responseMessage) =>
        new(GetHttpClientFactoryMock(responseMessage),
        new WeaponsParser(parserOptions.WeaponsOptions),
        GetResiliencePipeline()
    );

    protected override void ValidatePage(WeaponsExpectedPage expected, WeaponsPageDto actual)
    {
        Assert.NotEmpty(actual.Categories);
        Assert.Equal(expected.WeaponCount, actual.WeaponCount);
        Assert.Equal(expected.CategoryCount, actual.CategoryCount);

        var firstCategory = actual.Categories.First();

        Assert.Equal(expected.CategoryNames.First(), firstCategory.Category);
        Assert.NotEmpty(firstCategory.Weapons);
        Assert.Equal(expected.WeaponCountEachCategory.First(), firstCategory.WeaponInCategoryCount);
    }
}
