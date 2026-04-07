using System;
using CsgoDbSource.Dtos.GlovesDtos;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Tests.ExpectedData;
using Microsoft.AspNetCore.Http;

namespace CsgoDbSource.Tests.CsgoDbSource.Tests.Integration;

public class GlovesEndpointTests(TestsWebApplicationFactory factory) : BaseEndpointTests<GlovesPageDto>(factory), IClassFixture<TestsWebApplicationFactory>
{
    protected override void SuccessValidate(GlovesPageDto expected, GlovesPageDto actual)
    {
        Assert.Equal(expected.GloveCount, actual.GloveCount);
        Assert.Equal(expected.SkinCount, actual.SkinCount);
        Assert.NotEmpty(actual.Gloves);
    }


    private static readonly GlovesExpectedData glovesExpectedData = new();

    [Fact]
    public async Task GetGloves_Should_Succeed() =>
        await EndpointShouldSucceed("/gloves", glovesExpectedData.ToPageDto());

    [Fact]
    public async Task GetGloves_Should_Fail_502() =>
        await EndpointShouldFail(
            "/gloves",
            new SourceStructureException(BaseCsgoDbSourceException.SourceStructureProblem),
            StatusCodes.Status502BadGateway
        );

    [Fact]
    public async Task GetGloves_Should_Fail_499() =>
        await EndpointShouldFail("/gloves", new OperationCanceledException(), StatusCodes.Status499ClientClosedRequest);

    [Fact]
    public async Task GetGloves_Should_Fail_500() =>
        await EndpointShouldFail("/gloves", new("Unexpected"), StatusCodes.Status500InternalServerError);
}
