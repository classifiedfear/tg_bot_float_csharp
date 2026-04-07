using System.Text;
using CsgoDbSource.Dtos.AdditionalInfoDtos;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Parsers;
using CsgoDbSource.Tests.Fixtures;

namespace CsgoDbSource.Tests.Parsers;

[Collection("csgodb")]
public class AdditionalInfoParserTests(HtmlPagesFixture htmlPages, ParserOptionsFixture parserOptions)
{
    private readonly AdditionalInfoParser parser = new(parserOptions.AdditionalInfoOptions);
    private readonly static AdditionalInfoPageDto ExpectedAwpLightningStrikePage = new AdditionalInfoPageDto.Builder()
        .WithWeapon("AWP")
        .WithSkinName("Lightning Strike")
        .WithRarity("Covert")
        .WithQualities(["Factory New", "Minimal Wear"])
        .WithStattrakQualities(["Factory New", "Minimal Wear"])
        .WithStattrakExistence(true)
        .WithImg("https://www.csgodatabase.com/images/skins/webp/AWP_Lightning_Strike.webp")
        .Build();

    private readonly static AdditionalInfoPageDto ExpectedP90EmeraldDragonPage = new AdditionalInfoPageDto.Builder()
        .WithWeapon("P90")
        .WithSkinName("Emerald Dragon")
        .WithRarity("Classified")
        .WithQualities(["Minimal Wear", "Field-Tested", "Well-Worn"])
        .WithStattrakQualities(["Factory New", "Minimal Wear", "Field-Tested"])
        .WithStattrakExistence(true)
        .WithImg("https://www.csgodatabase.com/images/skins/webp/P90_Emerald_Dragon.webp")
        .Build();

    public static TheoryData<AdditionalInfoPageDto> ExpectedPage => new() {
         { ExpectedAwpLightningStrikePage },
         { ExpectedP90EmeraldDragonPage }
        };

    [Theory]
    [MemberData(nameof(ExpectedPage), DisableDiscoveryEnumeration = true)]
    public async Task GetParsedData_Should_Parse_Complete_Page(AdditionalInfoPageDto expectedPage)
    {
        using var stream = new MemoryStream(
            Encoding.UTF8.GetBytes(htmlPages.GetAdditionalInfoPage(expectedPage.WeaponName, expectedPage.SkinName))
        );

        AdditionalInfoPageDto actual = await parser.GetParsedData(stream, CancellationToken.None);

        Assert.NotNull(actual);
        Assert.Equal(expectedPage.WeaponName, actual.WeaponName);
        Assert.Equal(expectedPage.SkinName, actual.SkinName);
        Assert.Equal(expectedPage.Rarity, actual.Rarity);
        Assert.Equal(expectedPage.StattrakExistence, actual.StattrakExistence);
        Assert.Equal(expectedPage.WeaponSkinImg, actual.WeaponSkinImg);

        Assert.All(expectedPage.Qualities, quality => Assert.Contains(quality, actual.Qualities));
        Assert.All(expectedPage.StattrakQualities, quality => Assert.Contains(quality, actual.StattrakQualities));
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
