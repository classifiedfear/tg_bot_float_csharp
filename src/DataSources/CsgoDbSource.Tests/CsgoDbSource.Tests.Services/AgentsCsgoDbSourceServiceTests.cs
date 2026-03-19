using System;
using System.Net;
using CsgoDbSource.Dtos.AgentsDtos;
using CsgoDbSource.Parsers;
using CsgoDbSource.Services;
using CsgoDbSource.Tests.ExpectedResults;
using CsgoDbSource.Tests.Fixtures;

namespace CsgoDbSource.Tests.CsgoDbSource.Tests.Services;

[Collection("csgodb")]
public class AgentsCsgoDbSourceServiceTests(
    HtmlPagesFixture htmlPages,
    ParserOptionsFixture parserOptions
) : BaseCsgoDbServiceTests<AgentsExpectedPage, AgentsPageDto>
{
    private static readonly AgentsExpectedPage expectedPage = new();
    protected override CsgoDbSourceService<AgentsPageDto> CreateService(HttpResponseMessage responseMessage) =>
        new(
            GetHttpClientFactoryMock(responseMessage),
            new AgentsParser(parserOptions.AgentsOptions),
            GetResiliencePipeline()
        );

    [Fact]
    public async Task GetPage_Should_Return_Parsed_Page() =>
        await ReturnParsedPage(
            expectedPage,
            new() { StatusCode = HttpStatusCode.OK, Content = new StringContent(htmlPages.AgentsPage) }
        );

    [Fact]
    public async Task GetPage_Should_Throw_When_Cancellation_Requested() =>
        await CancellationRequested(
            new() { StatusCode = HttpStatusCode.OK, Content = new StringContent(htmlPages.AgentsPage) }
        );

    [Fact]
    public async Task GetPage_Should_Throw_When_Source_Changed() =>
        await SourceChanged(
            new() { StatusCode = HttpStatusCode.OK, Content = new StringContent(htmlPages.WrongPage) }
        );

    protected override void ValidatePage(AgentsExpectedPage expected, AgentsPageDto actual)
    {
        Assert.Equal(expected.AgentCount, actual.FractionCount);
        Assert.Equal(expected.SkinCount, actual.SkinsCount);
        Assert.NotEmpty(actual.Agents);

        var firstAgent = actual.Agents.First();
        Assert.Equal(expected.AgentNames.First(), firstAgent.FractionName);
        Assert.NotEmpty(firstAgent.Skins);
        Assert.Equal(expected.SkinCountEachAgent.First(), firstAgent.SkinsCount);
    }
}
