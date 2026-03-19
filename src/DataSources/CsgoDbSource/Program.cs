
using System.Net;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

using CsgoDbSource.Parsers;
using CsgoDbSource.Parsers.Options;
using CsgoDbSource.Services.Options;
using CsgoDbSource.Services;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Dtos.GlovesDtos;
using CsgoDbSource.Dtos.WeaponsDtos;
using CsgoDbSource.Dtos.SkinsDtos;
using CsgoDbSource.Dtos.AgentsDtos;
using CsgoDbSource.Dtos.AdditionalInfoDtos;
using CsgoDbSource.CsgoDbSource.Middlewares;
using Polly.Registry;




public partial class Program
{
    public static async Task Main(params string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureBuilder(builder);

        var app = builder.Build();
        ConfigureApp(app);
        await app.RunAsync();
    }

    private static void ConfigureBuilder(WebApplicationBuilder builder)
    {
        builder.WebHost.UseUrls("http://0.0.0.0:5002");
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddResiliencePipeline<string, HttpResponseMessage>("csgodb", static builder =>
            builder.AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .HandleResult(static result =>
                        result.StatusCode is not HttpStatusCode.Forbidden && !result.IsSuccessStatusCode)
            }));

        builder.Services.AddHttpClient();

        builder.Services.AddSingleton(
            sp => sp.GetRequiredService<ResiliencePipelineProvider<string>>()
            .GetPipeline<HttpResponseMessage>("csgodb")
        );

        builder.Services.Configure<RequestOptions>(
            builder.Configuration.GetSection(nameof(RequestOptions))
            );

        builder.Services.Configure<WeaponsParserOptions>(
            builder.Configuration.GetSection(nameof(WeaponsParser))
            );
        builder.Services.Configure<SkinsParserOptions>(
            builder.Configuration.GetSection(nameof(SkinsParser))
            );
        builder.Services.Configure<AdditionalInfoParserOptions>(
            builder.Configuration.GetSection(nameof(AdditionalInfoParser))
            );
        builder.Services.Configure<AgentsParserOptions>(
            builder.Configuration.GetSection(nameof(AgentsParser))
            );
        builder.Services.Configure<GlovesParserOptions>(
            builder.Configuration.GetSection(nameof(GlovesParser))
            );

        builder.Services.AddTransient<BaseParser<WeaponsPageDto>, WeaponsParser>();
        builder.Services.AddTransient<BaseParser<SkinsPageDto>, SkinsParser>();
        builder.Services.AddTransient<BaseParser<AdditionalInfoPageDto>, AdditionalInfoParser>();
        builder.Services.AddTransient<BaseParser<AgentsPageDto>, AgentsParser>();
        builder.Services.AddTransient<BaseParser<GlovesPageDto>, GlovesParser>();
        builder.Services.AddTransient(typeof(ICsgoDbSourceService<>), typeof(CsgoDbSourceService<>));
    }

    private static void ConfigureApp(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionMiddleware>();
        app.UseHttpsRedirection();
        MapRoutes(app);
    }

    private static void MapRoutes(WebApplication app)
    {
        app.MapGet(
            "/weapons",
            async (ICsgoDbSourceService<WeaponsPageDto> service, IOptions<RequestOptions> options, CancellationToken cancellationToken) =>
        {
            RequestOptions _options = options.Value;
            return Results.Ok(await service.GetPage(_options.BasePage + _options.WeaponsPage, cancellationToken));
        }).WithName("GetWeapons");

        app.MapGet(
            "/weapons/{weapon:required:minlength(3)}/skins",
            async (ICsgoDbSourceService<SkinsPageDto> service, IOptions<RequestOptions> options, CancellationToken cancellationToken, string weapon) =>
            {
                RequestOptions _options = options.Value;
                return Results.Ok(await service.GetPage(
                    string.Format(
                        _options.BasePage + _options.SkinsPage,
                        weapon.ToLower().Replace(' ', '-').Replace("★ ", "")),
                    cancellationToken
                ));
            }
        ).WithName("GetSkins");

        app.MapGet(
            "/weapons/{weapon:required:minlength(3)}/{skin:required}",
            async (ICsgoDbSourceService<AdditionalInfoPageDto> service, IOptions<RequestOptions> options, CancellationToken cancellationToken, string weapon, string skin) =>
            {
                RequestOptions _options = options.Value;
                return Results.Ok(await service.GetPage(
                    string.Format(
                        _options.BasePage + _options.AdditionalInfoPage,
                        weapon.ToLower().Replace(' ', '-'),
                        skin.ToLower().Replace(' ', '-')),
                    cancellationToken
                ));
            }
        );

        app.MapGet(
            "/agents",
            async (ICsgoDbSourceService<AgentsPageDto> service, IOptions<RequestOptions> options, CancellationToken cancellationToken) =>
            {
                RequestOptions _options = options.Value;
                return Results.Ok(await service.GetPage(
                    _options.BasePage + _options.AgentsPage,
                    cancellationToken
                ));
            }
        );

        app.MapGet(
            "/gloves",
            async (ICsgoDbSourceService<GlovesPageDto> service, IOptions<RequestOptions> options, CancellationToken cancellationToken) =>
            {
                RequestOptions _options = options.Value;
                return Results.Ok(await service.GetPage(
                    _options.BasePage + _options.GlovesPage,
                    cancellationToken
                ));
            }
        );
    }
}