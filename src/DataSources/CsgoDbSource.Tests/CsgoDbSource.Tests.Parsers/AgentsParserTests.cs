using System.Text;
using CsgoDbSource.Dtos.AgentsDtos;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Parsers;
using CsgoDbSource.Tests.ExpectedResults;
using CsgoDbSource.Tests.Fixtures;

namespace CsgoDbSource.Tests.Parsers;

[Collection("csgodb")]
public class AgentsParserTests(HtmlPagesFixture htmlPages, ParserOptionsFixture parserOptions)
{
    private readonly AgentsParser parser = new(parserOptions.AgentsOptions);

    private static readonly AgentsExpectedPage expectedPage = new()
    {
        ChosenAgentSkins = [
            AgentsExpectedPage.MakeAgent(
                "Cmdr. Frank 'Wet Sox' Baroud",
                "https://www.csgodatabase.com/images/agents/webp/Cmdr._Frank_'Wet_Sox'_Baroud_SEAL_Frogman.webp",
                "Master"
            ),
            AgentsExpectedPage.MakeAgent(
                "Lieutenant 'Tree Hugger' Farlow",
                "https://www.csgodatabase.com/images/agents/webp/Lieutenant_'Tree_Hugger'_Farlow_SWAT.webp",
                "Exceptional"
            ),
            AgentsExpectedPage.MakeAgent(
                "D Squadron Officer",
                "https://www.csgodatabase.com/images/agents/webp/D_Squadron_Officer_NZSAS.webp",
                "Distinguished"
            ),
            AgentsExpectedPage.MakeAgent(
                "Rezan The Ready",
                "https://www.csgodatabase.com/images/agents/webp/Rezan_The_Ready_Sabre.webp",
                "Superior"
            ),
            AgentsExpectedPage.MakeAgent(
                "'Blueberries' Buckshot",
                "https://www.csgodatabase.com/images/agents/webp/'Blueberries'_Buckshot_NSWC_SEAL.webp",
                "Exceptional"
            ),
            AgentsExpectedPage.MakeAgent(
                "Michael Syfers",
                "https://www.csgodatabase.com/images/agents/webp/Michael_Syfers_FBI_Sniper.webp",
                "Superior"
            ),
        ]
    };

    [Fact]
    public async Task GetParsedData_Should_Parse_Complete_Agents_Page()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlPages.AgentsPage));

        var actual = await parser.GetParsedData(stream, CancellationToken.None);

        Assert.NotNull(actual);
        Assert.Equal(expectedPage.AgentCount, actual.FractionCount);
        Assert.Equal(expectedPage.SkinCount, actual.SkinsCount);

        Assert.Equal(expectedPage.AgentNames, actual.Agents.Select(agentSkinDto => agentSkinDto.FractionName));
        Assert.Equal(expectedPage.SkinCountEachAgent, actual.Agents.Select(agentSkinDto => agentSkinDto.SkinsCount));

        var actualAgents = actual.Agents.SelectMany(
            agentSkinsDto => agentSkinsDto.Skins).Where(
                agentDto => expectedPage.ChosenAgentSkins.Any(
                    expectedDto => agentDto == expectedDto
                )
            );

        Assert.Equal(expectedPage.ChosenAgentSkins, actualAgents);
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
