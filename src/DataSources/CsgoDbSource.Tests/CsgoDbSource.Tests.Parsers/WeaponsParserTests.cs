using CsgoDbSource.Dtos.WeaponsDtos;
using CsgoDbSource.Parsers;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Tests.ExpectedResults;
using System.Text;
using CsgoDbSource.Tests.Fixtures;

namespace CsgoDbSource.Tests.Parsers;

[Collection("csgodb")]
public class WeaponsParserTests(HtmlPagesFixture htmlPages, ParserOptionsFixture optionsFixture)
{

    private readonly WeaponsParser parser = new(optionsFixture.WeaponsOptions);

    private static readonly WeaponsExpectedPage expectedPage = new()
    {
        OneWeaponEachCategory = [
            WeaponsExpectedPage.MakeWeapon("CZ75-Auto", "https://www.csgodatabase.com/images/weapons/webp/CZ75-Auto.webp", 36),
            WeaponsExpectedPage.MakeWeapon("AUG", "https://www.csgodatabase.com/images/weapons/webp/AUG.webp", 44),
            WeaponsExpectedPage.MakeWeapon("MAC-10", "https://www.csgodatabase.com/images/weapons/webp/MAC-10.webp", 55),
            WeaponsExpectedPage.MakeWeapon("XM1014", "https://www.csgodatabase.com/images/weapons/webp/XM1014.webp", 44),
            WeaponsExpectedPage.MakeWeapon("Falchion Knife", "https://www.csgodatabase.com/images/knives/webp/Falchion_Knife.webp", 25),
            WeaponsExpectedPage.MakeWeapon("Zeus x27", "https://www.csgodatabase.com/images/weapons/webp/Zeus_x27.webp", 7),
        ]
    };

    [Fact]
    public async Task GetParsedData_Should_Parse_Complete_Weapons_Page()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlPages.WeaponsPage));

        var actual = await parser.GetParsedData(stream, CancellationToken.None);

        Assert.NotNull(actual);
        Assert.Equal(expectedPage.CategoryCount, actual.CategoryCount);
        Assert.Equal(expectedPage.WeaponCount, actual.WeaponCount);

        Assert.Equal(expectedPage.CategoryNames, actual.Categories.Select(c => c.Category));
        Assert.Equal(expectedPage.WeaponCountEachCategory, actual.Categories.Select(c => c.WeaponInCategoryCount));

        var actualWeapons = actual.Categories.SelectMany(
            categoryDto => categoryDto.Weapons).Where(
                weaponDto => expectedPage.OneWeaponEachCategory.Any(
                    expectedWeapon => expectedWeapon == weaponDto)
            ).ToArray();

        Assert.Equal(expectedPage.OneWeaponEachCategory, actualWeapons);
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
