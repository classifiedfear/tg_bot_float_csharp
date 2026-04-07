using CsgoDbSource.Dtos.WeaponsDtos;
using CsgoDbSource.Parsers;
using CsgoDbSource.Services;
using Microsoft.VisualBasic;
using CsgoDbSource.Tests.Fixtures;
using CsgoDbSource.Tests.ExpectedData;
using System.Net;

namespace CsgoDbSource.Tests;

[Collection("csgodb")]
public class WeaponsCsgoDbSourceServiceTests(
    HtmlPagesFixture htmlPages, ParserOptionsFixture parserOptions
) : BaseCsgoDbServiceTests<WeaponsPageDto>
{
    private static readonly WeaponsExpectedData expectedData = new();
    private static readonly WeaponsPageDto expected = expectedData.ToPageDto();
    protected override CsgoDbSourceService<WeaponsPageDto> CreateService(HttpResponseMessage responseMessage) =>
        new(GetHttpClientFactoryMock(responseMessage),
        new WeaponsParser(parserOptions.WeaponsOptions),
        GetResiliencePipeline()
    );

    protected override void ValidatePage(WeaponsPageDto expected, WeaponsPageDto actual)
    {
        Assert.NotEmpty(actual.Categories);
        Assert.Equal(expected.WeaponCount, actual.WeaponCount);
        Assert.Equal(expected.CategoryCount, actual.CategoryCount);

        var actualFirstCategory = actual.Categories.First();
        var expectedFirstCategory = expected.Categories.First();

        Assert.Equal(expectedFirstCategory.Category, actualFirstCategory.Category);
        Assert.Equal(expectedFirstCategory.WeaponInCategoryCount, actualFirstCategory.WeaponInCategoryCount);
        Assert.NotEmpty(actualFirstCategory.Weapons);
    }

    [Fact]
    public async Task GetPage_Should_Return_Parsed_Page() =>
        await ReturnParsedPage(
            expected, new() { StatusCode = HttpStatusCode.OK, Content = new StringContent(htmlPages.WeaponsPage) }
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

}
