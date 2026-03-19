using System.Text;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Parsers;
using CsgoDbSource.Tests.ExpectedResults;
using CsgoDbSource.Tests.Fixtures;

namespace CsgoDbSource.Tests.Parsers;

[Collection("csgodb")]
public class SkinsParserTests(HtmlPagesFixture htmlPages, ParserOptionsFixture parserOptions)
{
    private readonly SkinsParser parser = new(parserOptions.SkinsOptions);

    private static readonly SkinsExpectedPage ExpectedFamasPage = new()
    {
        WeaponName = "FAMAS",
        Skins = [
                    SkinsExpectedPage.MakeSkin("Afterimage", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Afterimage.webp", "Classified"),
                    SkinsExpectedPage.MakeSkin("Spitfire", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Spitfire.webp", "Restricted"),
                    SkinsExpectedPage.MakeSkin("Byproduct", "https://www.csgodatabase.com/images/skins/webp/FAMAS_Byproduct.webp", "Consumer"),
                ],
        SkinCount = 42
    };

    private static readonly SkinsExpectedPage ExpectedDesertEaglePage = new()
    {
        WeaponName = "Desert Eagle",
        Skins = [
                    SkinsExpectedPage.MakeSkin("Golden Koi", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_Golden_Koi.webp", "Covert"),
                    SkinsExpectedPage.MakeSkin("Meteorite", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_Meteorite.webp", "Mil-spec"),
                    SkinsExpectedPage.MakeSkin("The Bronze", "https://www.csgodatabase.com/images/skins/webp/Desert_Eagle_The_Bronze.webp", "Industrial"),
                ],
        SkinCount = 43
    };


    private static readonly SkinsExpectedPage ExpectedKarambitPage = new()
    {
        WeaponName = "Karambit",
        Skins = [
                    SkinsExpectedPage.MakeSkin("Autotronic", "https://www.csgodatabase.com/images/knives/webp/Karambit_Autotronic.webp", "Extraordinary"),
                    SkinsExpectedPage.MakeSkin("Fade", "https://www.csgodatabase.com/images/knives/webp/Karambit_Fade.webp", "Extraordinary"),
                    SkinsExpectedPage.MakeSkin("Ultraviolet", "https://www.csgodatabase.com/images/knives/webp/Karambit_Ultraviolet.webp", "Extraordinary"),
                ],
        SkinCount = 25
    };

    public static TheoryData<SkinsExpectedPage> ExpectedOnOnePage => new() { { ExpectedFamasPage }, { ExpectedDesertEaglePage }, { ExpectedKarambitPage } };

    [Theory]
    [MemberData(nameof(ExpectedOnOnePage), DisableDiscoveryEnumeration = true)]
    public async Task GetParsedData_Should_Parse_Complete_Skins_Page(SkinsExpectedPage expectedPage)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlPages.GetSkinPage(expectedPage.WeaponName)));

        var actual = await parser.GetParsedData(stream, CancellationToken.None);

        Assert.NotNull(actual);
        Assert.Equal(expectedPage.WeaponName, actual.WeaponName);
        Assert.Equal(expectedPage.SkinCount, actual.SkinCount);

        Assert.All(expectedPage.Skins, skinDto => Assert.Contains(skinDto, actual.Skins));
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
