using System;
using System.Net;
using CsgoDbSource.Dtos.AgentsDtos;
using CsgoDbSource.Parsers;
using CsgoDbSource.Services;
using CsgoDbSource.Tests.ExpectedData;
using CsgoDbSource.Tests.Fixtures;

namespace CsgoDbSource.Tests.CsgoDbSource.Tests.Services;

[Collection("csgodb")]
public class AgentsCsgoDbSourceServiceTests(
    HtmlPagesFixture htmlPages,
    ParserOptionsFixture parserOptions
) : BaseCsgoDbServiceTests<AgentsPageDto>
{
    private static readonly AgentsExpectedData expectedPage = new();
    private static readonly AgentsPageDto expected = expectedPage.ToPageDto();
    protected override CsgoDbSourceService<AgentsPageDto> CreateService(HttpResponseMessage responseMessage) =>
        new(
            GetHttpClientFactoryMock(responseMessage),
            new AgentsParser(parserOptions.AgentsOptions),
            GetResiliencePipeline()
        );

    [Fact]
    public async Task GetPage_Should_Return_Parsed_Page() =>
        await ReturnParsedPage(
            expected,
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

    protected override void ValidatePage(AgentsPageDto expected, AgentsPageDto actual)
    {
        Assert.Equal(expected.AgentCount, actual.AgentCount);
        Assert.Equal(expected.SkinCount, actual.SkinCount);
        Assert.NotEmpty(actual.Agents);

        var actualLastAgent = actual.Agents.Last();
        var expectedLastAgent = expected.Agents.Last();

        Assert.Equal(expectedLastAgent.AgentName, actualLastAgent.AgentName);
        Assert.Equal(expectedLastAgent.SkinCount, actualLastAgent.SkinCount);
        Assert.NotEmpty(actualLastAgent.Skins);
    }
}
