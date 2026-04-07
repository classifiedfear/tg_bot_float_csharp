using System.Text;
using CsgoDbSource.Dtos.GlovesDtos;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Parsers;
using CsgoDbSource.Tests.ExpectedData;
using CsgoDbSource.Tests.Fixtures;

namespace CsgoDbSource.Tests.Parsers;

[Collection("csgodb")]
public class GlovesParserTests(HtmlPagesFixture htmlPages, ParserOptionsFixture parserOptions)
{
    private readonly GlovesParser parser = new(parserOptions.GlovesOptions);
    private static readonly GlovesExpectedData expectedData = new()
    {
        SampleSkinEachGlove = [
            GlovesExpectedData.MakeGlove(
                "Charred",
                "https://www.csgodatabase.com/images/gloves/webp/Bloodhound_Gloves_Charred.webp",
                "Extraordinary"
            ),
            GlovesExpectedData.MakeGlove(
                "Jade",
                "https://www.csgodatabase.com/images/gloves/webp/Broken_Fang_Gloves_Jade.webp",
                "Extraordinary"
            ),
            GlovesExpectedData.MakeGlove(
                "Diamondback",
                "https://www.csgodatabase.com/images/gloves/webp/Driver_Gloves_Diamondback.webp",
                "Extraordinary"
            ),
            GlovesExpectedData.MakeGlove(
                "CAUTION!",
                "https://www.csgodatabase.com/images/gloves/webp/Hand_Wraps_CAUTION!.webp",
                "Extraordinary"
            ),
            GlovesExpectedData.MakeGlove(
                "Emerald",
                "https://www.csgodatabase.com/images/gloves/webp/Hydra_Gloves_Emerald.webp",
                "Extraordinary"
            ),
            GlovesExpectedData.MakeGlove(
                "Polygon",
                "https://www.csgodatabase.com/images/gloves/webp/Moto_Gloves_Polygon.webp",
                "Extraordinary"
            ),
            GlovesExpectedData.MakeGlove(
                "Buckshot",
                "https://www.csgodatabase.com/images/gloves/webp/Specialist_Gloves_Buckshot.webp",
                "Extraordinary"
            ),
            GlovesExpectedData.MakeGlove(
                "Arid",
                "https://www.csgodatabase.com/images/gloves/webp/Sport_Gloves_Arid.webp",
                "Extraordinary")]
    };

    private readonly static GlovesPageDto expected = expectedData.ToPageDto();

    [Fact]
    public async Task GetParsedData_Should_Parse_Complete_Gloves_Page()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlPages.GlovesPage));

        var actual = await parser.GetParsedData(stream, CancellationToken.None);

        Assert.NotNull(actual);
        Assert.Equal(expected.GloveCount, actual.GloveCount);
        Assert.Equal(expected.SkinCount, actual.SkinCount);

        var expectedAndActualGloveSkinsDtos = expected.Gloves.Zip(actual.Gloves);

        foreach (
            (
                GloveSkinsDto expectedGloveSkinsDto,
                GloveSkinsDto actualGloveSkinsDto
            ) in expectedAndActualGloveSkinsDtos)
        {
            Assert.Equal(expectedGloveSkinsDto.GloveName, actualGloveSkinsDto.GloveName);
            Assert.Equal(expectedGloveSkinsDto.SkinCount, actualGloveSkinsDto.SkinCount);

            Assert.NotEmpty(actualGloveSkinsDto.Skins);

            var sampleGloveSkin = expectedGloveSkinsDto.Skins[0];

            Assert.Contains(sampleGloveSkin, actualGloveSkinsDto.Skins);
        }
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
