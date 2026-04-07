using System.Text;
using CsgoDbSource.Dtos.AgentsDtos;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Parsers;
using CsgoDbSource.Tests.ExpectedData;
using CsgoDbSource.Tests.Fixtures;

namespace CsgoDbSource.Tests.Parsers;

[Collection("csgodb")]
public class AgentsParserTests(HtmlPagesFixture htmlPages, ParserOptionsFixture parserOptions)
{
    private readonly AgentsParser parser = new(parserOptions.AgentsOptions);

    private static readonly AgentsExpectedData expectedData = new();
    private static readonly AgentsPageDto expected = expectedData.ToPageDto();

    [Fact]
    public async Task GetParsedData_Should_Parse_Complete_Agents_Page()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlPages.AgentsPage));

        var actual = await parser.GetParsedData(stream, CancellationToken.None);

        Assert.NotNull(actual);
        Assert.Equal(expected.AgentCount, actual.AgentCount);
        Assert.Equal(expected.SkinCount, actual.SkinCount);

        var expectedAndActualAgentSkinsDtos = expected.Agents.Zip(actual.Agents);

        foreach (
            (
                AgentSkinsDto expectedAgentSkinsDto,
                AgentSkinsDto actualAgentSkinsDto
            ) in expectedAndActualAgentSkinsDtos)
        {
            Assert.Equal(expectedAgentSkinsDto.AgentName, actualAgentSkinsDto.AgentName);
            Assert.Equal(expectedAgentSkinsDto.SkinCount, actualAgentSkinsDto.SkinCount);

            Assert.NotEmpty(actualAgentSkinsDto.Skins);

            var sampleAgentSkin = expectedAgentSkinsDto.Skins[0];

            Assert.Contains(sampleAgentSkin, actualAgentSkinsDto.Skins);
        }
    }

    [Fact]
    public async Task GetParsedData_Should_Throw_SourceStructureException()
    {
        using var stream = new MemoryStream();

        var exception = await Assert.ThrowsAsync<SourceStructureException>(
            async () => await parser.GetParsedData(stream, CancellationToken.None)
        );

        Assert.Equal(BaseCsgoDbSourceException.SourceStructureProblem, exception.Message);
    }
}
