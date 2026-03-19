using System;
using CsgoDbSource.Dtos.GlovesDtos;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Tests.ExpectedResults;
using Microsoft.AspNetCore.Http;

namespace CsgoDbSource.Tests.CsgoDbSource.Tests.Integration;

public class GlovesEndpointTests(TestsWebApplicationFactory factory) : BaseEndpointTests<GlovesPageDto>(factory), IClassFixture<TestsWebApplicationFactory>
{
    protected override void SuccessValidate(GlovesPageDto expected, GlovesPageDto actual)
    {
        Assert.Equal(expected.GlovesCount, actual.GlovesCount);
        Assert.Equal(expected.SkinsCount, actual.SkinsCount);
        Assert.NotEmpty(actual.Gloves);

        var firstCategory = actual.Gloves.First();
        var expectedFirstCategory = expected.Gloves.First();
        Assert.Equal(expectedFirstCategory.GloveName, firstCategory.GloveName);
        Assert.Equal(expectedFirstCategory.SkinCount, firstCategory.SkinCount);
        Assert.NotEmpty(firstCategory.Skins);
    }


    private static readonly GlovesExpectedPage glovesPageData = new();

    private static GlovesPageDto CreateExpectedPage(GlovesExpectedPage expectedData)
    {
        var glovesSkinDtos = new List<GloveSkinsDto>();
        for (int i = 0; i < expectedData.GloveCount; i++)
        {
            glovesSkinDtos.Add(
                new()
                {
                    GloveName = expectedData.GloveNames[i],
                    Skins = [expectedData.OneSkinEachGlove[i]]
                });
        }

        return new() { Gloves = glovesSkinDtos, SkinsCount = expectedData.SkinCount };
    }

    [Fact]
    public async Task GetGloves_Should_Succeed() =>
        await EndpointShouldSucceed("/gloves", CreateExpectedPage(glovesPageData));

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
