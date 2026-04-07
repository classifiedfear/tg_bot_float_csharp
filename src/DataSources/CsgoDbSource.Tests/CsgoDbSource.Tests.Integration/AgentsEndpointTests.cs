using System;
using CsgoDbSource.Dtos.AgentsDtos;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Tests.ExpectedData;
using Microsoft.AspNetCore.Http;

namespace CsgoDbSource.Tests.CsgoDbSource.Tests.Integration;

public class AgentsEndpointTests(TestsWebApplicationFactory factory) :
    BaseEndpointTests<AgentsPageDto>(factory), IClassFixture<TestsWebApplicationFactory>
{
    protected override void SuccessValidate(AgentsPageDto expected, AgentsPageDto actual)
    {
        Assert.Equal(expected.AgentCount, actual.AgentCount);
        Assert.Equal(expected.SkinCount, actual.SkinCount);
        Assert.NotEmpty(actual.Agents);
    }

    private static readonly AgentsExpectedData agentsExpecteData = new();


    [Fact]
    public async Task GetAgents_Should_Succeed() =>
        await EndpointShouldSucceed("/agents", agentsExpecteData.ToPageDto());

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
