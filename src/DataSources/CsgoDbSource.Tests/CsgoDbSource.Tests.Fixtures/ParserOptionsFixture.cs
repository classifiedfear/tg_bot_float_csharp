using System;
using System.Runtime.InteropServices;
using CsgoDbSource.Parsers;
using CsgoDbSource.Parsers.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CsgoDbSource.Tests.Fixtures;

public class ParserOptionsFixture
{
    public IOptions<WeaponsParserOptions> WeaponsOptions { get; }
    public IOptions<SkinsParserOptions> SkinsOptions { get; }
    public IOptions<GlovesParserOptions> GlovesOptions { get; }
    public IOptions<AgentsParserOptions> AgentsOptions { get; }
    public IOptions<AdditionalInfoParserOptions> AdditionalInfoOptions { get; }

    public ParserOptionsFixture()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        WeaponsOptions = Options.Create(
            config.GetSection(nameof(WeaponsParser)).Get<WeaponsParserOptions>()!
        );

        SkinsOptions = Options.Create(
            config.GetSection(nameof(SkinsParser)).Get<SkinsParserOptions>()!
        );

        GlovesOptions = Options.Create(
            config.GetSection(nameof(GlovesParser)).Get<GlovesParserOptions>()!
        );

        AgentsOptions = Options.Create(
            config.GetSection(nameof(AgentsParser)).Get<AgentsParserOptions>()!
        );

        AdditionalInfoOptions = Options.Create(
            config.GetSection(nameof(AdditionalInfoParser)).Get<AdditionalInfoParserOptions>()!
        );


    }
}
