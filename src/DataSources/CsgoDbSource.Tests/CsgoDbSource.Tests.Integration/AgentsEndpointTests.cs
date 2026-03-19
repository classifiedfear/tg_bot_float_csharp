using System;
using CsgoDbSource.Dtos.AgentsDtos;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Tests.ExpectedResults;
using Microsoft.AspNetCore.Http;

namespace CsgoDbSource.Tests.CsgoDbSource.Tests.Integration;

public class AgentsEndpointTests(TestsWebApplicationFactory factory) :
    BaseEndpointTests<AgentsPageDto>(factory), IClassFixture<TestsWebApplicationFactory>
{
    protected override void SuccessValidate(AgentsPageDto expected, AgentsPageDto actual)
    {
        Assert.Equal(expected.FractionCount, actual.FractionCount);
        Assert.Equal(expected.SkinsCount, actual.SkinsCount);
        Assert.NotEmpty(actual.Agents);

        var firstCategory = actual.Agents.First();
        var expectedFirstCategory = expected.Agents.First();
        Assert.Equal(expectedFirstCategory.FractionName, firstCategory.FractionName);
        Assert.Equal(expectedFirstCategory.SkinsCount, firstCategory.SkinsCount);
    }


    private static readonly AgentsExpectedPage agentsPageData = new();

    private static AgentsPageDto CreateExpectedPage(AgentsExpectedPage expectedData)
    {
        var agentsSkinDtos = new List<AgentSkinsDto>();
        for (int i = 0; i < expectedData.AgentCount; i++)
        {
            agentsSkinDtos.Add(
                new()
                {
                    FractionName = expectedData.AgentNames[i],
                });
        }

        return new() { Agents = agentsSkinDtos, SkinsCount = expectedData.AgentCount };
    }

    [Fact]
    public async Task GetAgents_Should_Succeed() =>
        await EndpointShouldSucceed("/agents", CreateExpectedPage(agentsPageData));

    [Fact]
    public async Task GetAgents_Should_Fail_502() =>
        await EndpointShouldFail(
            "/agents",
            new SourceStructureException(BaseCsgoDbSourceException.SourceStructureProblem),
            StatusCodes.Status502BadGateway
        );

    [Fact]
    public async Task GetAgents_Should_Fail_499() =>
        await EndpointShouldFail("/agents", new OperationCanceledException(), StatusCodes.Status499ClientClosedRequest);

    [Fact]
    public async Task GetAgents_Should_Fail_500() =>
        await EndpointShouldFail("/agents", new("Unexpected"), StatusCodes.Status500InternalServerError);
}
