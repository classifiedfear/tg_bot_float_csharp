
using System.Net;
using System.Net.Http.Json;
using CsgoDbSource.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace CsgoDbSource.Tests.CsgoDbSource.Tests.Integration;

public abstract class BaseEndpointTests<T>(TestsWebApplicationFactory factory)
{
    protected readonly TestsWebApplicationFactory Factory = factory;

    protected abstract void SuccessValidate(T expected, T actual);

    protected Mock<ICsgoDbSourceService<T>> GetServiceMockSuccess(T expectedPage)
    {
        var mockService = new Mock<ICsgoDbSourceService<T>>();

        mockService
            .Setup(
                service => service.GetPage(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPage);

        return mockService;
    }

    protected Mock<ICsgoDbSourceService<T>> GetServiceMockUnsuccess(Exception exception)
    {
        var mockService = new Mock<ICsgoDbSourceService<T>>();

        mockService
            .Setup(service => service.GetPage(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        return mockService;
    }

    protected async Task EndpointShouldSucceed(string endpoint, T expectedPage)
    {
        Mock<ICsgoDbSourceService<T>> serviceMock = GetServiceMockSuccess(expectedPage);

        HttpClient client = Factory.CreateClientWithService(serviceMock.Object);

        HttpResponseMessage response = await client.GetAsync(endpoint);

        response.EnsureSuccessStatusCode();

        T? deserializedResponse = await response.Content.ReadFromJsonAsync<T>();

        Assert.NotNull(deserializedResponse);
        SuccessValidate(expectedPage, deserializedResponse);

        serviceMock.Verify(service => service.GetPage(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    protected async Task EndpointShouldFail(string endpoint, Exception exception, int status)
    {
        Mock<ICsgoDbSourceService<T>> serviceMock = GetServiceMockUnsuccess(exception);

        HttpClient client = Factory.CreateClientWithService(serviceMock.Object);

        HttpResponseMessage response = await client.GetAsync(endpoint);

        Assert.Equal((HttpStatusCode)status, response.StatusCode);

        serviceMock.Verify(service => service.GetPage(It.IsAny<string>(), It.IsAny<CancellationToken>()));
    }

}
