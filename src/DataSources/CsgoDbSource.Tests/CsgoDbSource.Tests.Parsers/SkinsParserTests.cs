using System.Text;
using CsgoDbSource.Dtos.SkinsDtos;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Parsers;
using CsgoDbSource.Tests.ExpectedData;
using CsgoDbSource.Tests.Fixtures;

namespace CsgoDbSource.Tests.Parsers;

[Collection("csgodb")]
public class SkinsParserTests(HtmlPagesFixture htmlPages, ParserOptionsFixture parserOptions)
{
    private readonly SkinsParser parser = new(parserOptions.SkinsOptions);

    private static readonly SkinsExpectedData ExpectedFamasPage = new()
    {
        WeaponName = "FAMAS",
        Skins = [
                    SkinsExpectedData.MakeSkin("Afterimage", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Afterimage.webp", "Classified"),
                    SkinsExpectedData.MakeSkin("Spitfire", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Spitfire.webp", "Restricted"),
                    SkinsExpectedData.MakeSkin("Byproduct", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Byproduct.webp", "Consumer"),
                ],
        SkinCount = 42
    };

    private static readonly SkinsExpectedData ExpectedDesertEaglePage = new()
    {
        WeaponName = "Desert Eagle",
        Skins = [
                    SkinsExpectedData.MakeSkin("Golden Koi", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_Golden_Koi.webp", "Covert"),
                    SkinsExpectedData.MakeSkin("Meteorite", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_Meteorite.webp", "Mil-spec"),
                    SkinsExpectedData.MakeSkin("The Bronze", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_The_Bronze.webp", "Industrial"),
                ],
        SkinCount = 43
    };


    private static readonly SkinsExpectedData ExpectedKarambitPage = new()
    {
        WeaponName = "Karambit",
        Skins = [
                    SkinsExpectedData.MakeSkin("Autotronic", "https://www.csgodatabase.com/images/knives/webp/Karambit_Autotronic.webp", "Extraordinary"),
                    SkinsExpectedData.MakeSkin("Fade", "https://www.csgodatabase.com/images/knives/webp/Karambit_Fade.webp", "Extraordinary"),
                    SkinsExpectedData.MakeSkin("Ultraviolet", "https://www.csgodatabase.com/images/knives/webp/Karambit_Ultraviolet.webp", "Extraordinary"),
                ],
        SkinCount = 24
    };

    public static TheoryData<SkinsPageDto> ExpectedPages => new() { { ExpectedFamasPage.ToPageDto() }, { ExpectedDesertEaglePage.ToPageDto() }, { ExpectedKarambitPage.ToPageDto() } };

    [Theory]
    [MemberData(nameof(ExpectedPages), DisableDiscoveryEnumeration = true)]
    public async Task GetParsedData_Should_Parse_Complete_Skins_Page(SkinsPageDto expected)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlPages.GetSkinPage(expected.WeaponName)));

        var actual = await parser.GetParsedData(stream, CancellationToken.None);

        Assert.NotNull(actual);

        Assert.Equal(expected.WeaponName, actual.WeaponName);
        Assert.Equal(expected.SkinCount, actual.SkinCount);

        Assert.All(expected.Skins, skinDto => Assert.Contains(skinDto, actual.Skins));
    }

    [Fact]
    public async Task GetParsedData_Should_Throw_PageException()
    {
        using var stream = new MemoryStream();

        var exception = await Assert.ThrowsAsync<PageException>(
            async () => await parser.GetParsedData(stream, CancellationToken.None)
        );
        Assert.Equal(BaseCsgoDbSourceException.NotFound, exception.Message);
    }

    [Fact]
    public async Task GetParsedData_Should_Throw_SourceStructureException()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlPages.WrongPage));

        var exception = await Assert.ThrowsAsync<SourceStructureException>(
            async () => await parser.GetParsedData(stream, CancellationToken.None)
        );
        Assert.Equal(BaseCsgoDbSourceException.SourceStructureProblem, exception.Message);
    }
}
