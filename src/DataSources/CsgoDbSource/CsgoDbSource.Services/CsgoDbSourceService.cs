using CsgoDbSource.Parsers;
using Polly;

namespace CsgoDbSource.Services;

public class CsgoDbSourceService<T>(
    IHttpClientFactory clientFactory,
    BaseParser<T> parser,
    ResiliencePipeline<HttpResponseMessage> pipeline
    ) : ICsgoDbSourceService<T>
{
    public async Task<T> GetPage(string url, CancellationToken cancellationToken)
    {

        var client = clientFactory.CreateClient();

        var response = await pipeline.ExecuteAsync(
            async token => await client.GetAsync(
                url,
                HttpCompletionOption.ResponseHeadersRead,
                token
            ), cancellationToken
        );

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await parser.GetParsedData(stream, cancellationToken);
    }
}
