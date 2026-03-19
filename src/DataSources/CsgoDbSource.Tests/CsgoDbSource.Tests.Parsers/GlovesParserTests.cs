using System.Text;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Parsers;
using CsgoDbSource.Tests.ExpectedResults;
using CsgoDbSource.Tests.Fixtures;

namespace CsgoDbSource.Tests.Parsers;

[Collection("csgodb")]
public class GlovesParserTests(HtmlPagesFixture htmlPages, ParserOptionsFixture parserOptions)
{
    private readonly GlovesParser parser = new(parserOptions.GlovesOptions);
    private static readonly GlovesExpectedPage expectedPage = new()
    {
        OneSkinEachGlove = [
            GlovesExpectedPage.MakeGlove(
                "Charred",
                "https://www.csgodatabase.com/images/gloves/webp/Bloodhound_Gloves_Charred.webp",
                "Extraordinary"
            ),
            GlovesExpectedPage.MakeGlove(
                "Jade",
                "https://www.csgodatabase.com/images/gloves/webp/Broken_Fang_Gloves_Jade.webp",
                "Extraordinary"
            ),
            GlovesExpectedPage.MakeGlove(
                "Diamondback",
                "https://www.csgodatabase.com/images/gloves/webp/Driver_Gloves_Diamondback.webp",
                "Extraordinary"
            ),
            GlovesExpectedPage.MakeGlove(
                "CAUTION!",
                "https://www.csgodatabase.com/images/gloves/webp/Hand_Wraps_CAUTION!.webp",
                "Extraordinary"
            ),
            GlovesExpectedPage.MakeGlove(
                "Emerald",
                "https://www.csgodatabase.com/images/gloves/webp/Hydra_Gloves_Emerald.webp",
                "Extraordinary"
            ),
            GlovesExpectedPage.MakeGlove(
                "Polygon",
                "https://www.csgodatabase.com/images/gloves/webp/Moto_Gloves_Polygon.webp",
                "Extraordinary"
            ),
            GlovesExpectedPage.MakeGlove(
                "Buckshot",
                "https://www.csgodatabase.com/images/gloves/webp/Specialist_Gloves_Buckshot.webp",
                "Extraordinary"
            ),
            GlovesExpectedPage.MakeGlove(
                "Arid",
                "https://www.csgodatabase.com/images/gloves/webp/Sport_Gloves_Arid.webp",
                "Extraordinary")
    ]
    };

    [Fact]
    public async Task GetParsedData_Should_Parse_Complete_Gloves_Page()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlPages.GlovesPage));

        var actual = await parser.GetParsedData(stream, CancellationToken.None);

        Assert.NotNull(actual);
        Assert.Equal(expectedPage.GloveCount, actual.GlovesCount);
        Assert.Equal(expectedPage.SkinCount, actual.SkinsCount);

        Assert.Equal(expectedPage.GloveNames, actual.Gloves.Select(gloveSkinDto => gloveSkinDto.GloveName));
        Assert.Equal(expectedPage.SkinCountEachGlove, actual.Gloves.Select(gloveSkinDto => gloveSkinDto.SkinCount));

        var actualGloves = actual.Gloves.SelectMany(
            gloveSkinDto => gloveSkinDto.Skins).Where(
                gloveDto => expectedPage.OneSkinEachGlove.Any(
                    expectedGlove => expectedGlove == gloveDto)
        );

        Assert.Equal(expectedPage.OneSkinEachGlove, actualGloves);
    }

    [Fact]
    public async Task GetParsedData_Should_Throw_SourceStructureException()
    {
        using var stream = new MemoryStream();

        var exception = await Assert.ThrowsAsync<SourceStructureException>(
            () => parser.GetParsedData(stream, CancellationToken.None)
        );

        Assert.Equal(BaseCsgoDbSourceException.SourceStructureProblem, exception.Message);
    }

}
