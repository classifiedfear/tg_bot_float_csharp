using System;

namespace CsgoDbSource.Services;

public interface ICsgoDbSourceService<T>
{
    Task<T> GetPage(string url, CancellationToken cancellationToken);
}
