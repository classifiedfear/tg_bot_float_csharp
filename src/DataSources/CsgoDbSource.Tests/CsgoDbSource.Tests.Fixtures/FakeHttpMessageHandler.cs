using System;
using System.Net;

namespace CsgoDbSource.Tests;

public class FakeHttpMessageHandler(HttpResponseMessage responseMessage) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        Task.FromResult(responseMessage);
}
