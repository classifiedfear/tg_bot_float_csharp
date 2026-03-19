using System;
using CsgoDbSource.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CsgoDbSource.Tests;

public class TestsWebApplicationFactory : WebApplicationFactory<Program>
{
    public HttpClient CreateClientWithService<T>(ICsgoDbSourceService<T> service) =>
        WithWebHostBuilder(
            builder => builder.ConfigureServices(
                services =>
                {
                    services.RemoveAll<ICsgoDbSourceService<T>>();
                    services.AddSingleton(service);
                }
            )
        ).CreateClient();
}
