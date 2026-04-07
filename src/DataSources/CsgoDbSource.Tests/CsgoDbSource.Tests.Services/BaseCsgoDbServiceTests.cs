using System.Net;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Services;
using Moq;
using Polly;
using Polly.Retry;

namespace CsgoDbSource.Tests;

public abstract class BaseCsgoDbServiceTests<T>
{
    protected static IHttpClientFactory GetHttpClientFactoryMock(HttpResponseMessage responseMessage)
    {
        HttpClient httpClient = new(new FakeHttpMessageHandler(responseMessage));
        Mock<IHttpClientFactory> factoryMock = new();

        factoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        return factoryMock.Object;
    }

    protected static ResiliencePipeline<HttpResponseMessage> GetResiliencePipeline()
    {
        var pipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = 1
            })
            .Build();
        return pipeline;
    }

    protected async Task ReturnParsedPage(T expected, HttpResponseMessage responseMessage)
    {
        var service = CreateService(responseMessage);

        var actual = await service.GetPage("https://test", CancellationToken.None);

        Assert.NotNull(actual);

        ValidatePage(expected, actual);
    }

    protected async Task CancellationRequested(HttpResponseMessage responseMessage)
    {
        var service = CreateService(responseMessage);

        using (var cts = new CancellationTokenSource())
        {
            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(
                () => service.GetPage("https://test", cts.Token)
            );
        }
    }

    protected async Task SourceChanged(HttpResponseMessage responseMessage)
    {
        var service = CreateService(responseMessage);

        await Assert.ThrowsAsync<SourceStructureException>(
            () => service.GetPage("https://test", CancellationToken.None)
        );
    }

    protected abstract ICsgoDbSourceService<T> CreateService(HttpResponseMessage responseMessage);
    protected abstract void ValidatePage(T expected, T actual);
}
