using CsgoDbSource.Dtos.WeaponsDtos;
using CsgoDbSource.Parsers;
using CsgoDbSource.Exceptions;

using System.Text;
using CsgoDbSource.Tests.Fixtures;
using CsgoDbSource.Tests.ExpectedData;

namespace CsgoDbSource.Tests.Parsers;

[Collection("csgodb")]
public class WeaponsParserTests(HtmlPagesFixture htmlPages, ParserOptionsFixture optionsFixture)
{

    private readonly WeaponsParser parser = new(optionsFixture.WeaponsOptions);

    private static readonly WeaponsExpectedData expectedData = new()
    {
        SampleWeaponPerCategory = [
            WeaponsExpectedData.MakeWeapon("CZ75-Auto", "https://www.csgodatabase.com/images/weapons/webp/CZ75-Auto.webp", 36),
            WeaponsExpectedData.MakeWeapon("AUG", "https://www.csgodatabase.com/images/weapons/webp/AUG.webp", 44),
            WeaponsExpectedData.MakeWeapon("MAC-10", "https://www.csgodatabase.com/images/weapons/webp/MAC-10.webp", 55),
            WeaponsExpectedData.MakeWeapon("XM1014", "https://www.csgodatabase.com/images/weapons/webp/XM1014.webp", 44),
            WeaponsExpectedData.MakeWeapon("Falchion Knife", "https://www.csgodatabase.com/images/knives/webp/Falchion_Knife.webp", 25),
            WeaponsExpectedData.MakeWeapon("Zeus x27", "https://www.csgodatabase.com/images/weapons/webp/Zeus_x27.webp", 7),
        ]
    };

    private static readonly WeaponsPageDto expected = expectedData.ToPageDto();

    [Fact]
    public async Task GetParsedData_Should_Parse_Complete_Weapons_Page()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlPages.WeaponsPage));

        var actual = await parser.GetParsedData(stream, CancellationToken.None);

        Assert.NotNull(actual);

        Assert.Equal(expected.CategoryCount, actual.CategoryCount);
        Assert.Equal(expected.WeaponCount, actual.WeaponCount);

        Assert.NotEmpty(actual.Categories);

        var expectedAndActualCategoryDtos = expected.Categories.Zip(actual.Categories);

        foreach (
            (
                CategoryWeaponDto expectedCategoryDto,
                CategoryWeaponDto actualCategoryDto
            ) in expectedAndActualCategoryDtos)
        {
            Assert.Equal(expectedCategoryDto.Category, actualCategoryDto.Category);
            Assert.Equal(expectedCategoryDto.WeaponInCategoryCount, actualCategoryDto.WeaponInCategoryCount);

            Assert.NotEmpty(actualCategoryDto.Weapons);

            var sampleWeapon = expectedCategoryDto.Weapons[0];

            Assert.Contains(sampleWeapon, actualCategoryDto.Weapons);
        }
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
