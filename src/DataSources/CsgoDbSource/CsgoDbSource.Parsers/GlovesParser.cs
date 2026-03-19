using System;
using System.Text.RegularExpressions;
using CsgoDbSource.Dtos;
using CsgoDbSource.Dtos.GlovesDtos;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Parsers.Options;
using CsgoDbSource.Parsers.ParserStates;
using Microsoft.Extensions.Options;

namespace CsgoDbSource.Parsers;


public sealed class GlovesParser : BaseParser<GlovesPageDto>
{
    private Regex GloveRegex { get; init; }
    private Regex SkinRegex { get; init; }
    private Regex RarityRegex { get; init; }
    private Regex ImgRegex { get; init; }

    private class CurrentDto
    {
        public string? GloveName { get; set; }
        public GloveDto.Builder Builder { get; } = new();
    }

    public GlovesParser(IOptions<GlovesParserOptions> options)
    {
        var _options = options.Value;
        GloveRegex = new(_options.GloveRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        SkinRegex = new(_options.SkinRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        RarityRegex = new(_options.RarityRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        ImgRegex = new(_options.ImgRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
    }

    public override async Task<GlovesPageDto> GetParsedData(Stream stream, CancellationToken cancellationToken)
    {
        using var streamReader = new StreamReader(stream);

        List<GloveSkinsDto> gloveSkinsDtos = await GetGlovesSkinsDto(streamReader, cancellationToken);

        if (gloveSkinsDtos.Count == 0)
            throw new SourceStructureException(BaseCsgoDbSourceException.SourceStructureProblem);

        return new() { Gloves = gloveSkinsDtos, SkinsCount = gloveSkinsDtos.Sum(dto => dto.SkinCount) };

    }

    private async Task<List<GloveSkinsDto>> GetGlovesSkinsDto(StreamReader streamReader, CancellationToken cancellationToken)
    {
        Dictionary<string, GloveSkinsDto> relations = [];

        CurrentDto dto = new();

        GlovesParserState state = GlovesParserState.FillNewDto;

        string? line;
        while ((line = await streamReader.ReadLineAsync(cancellationToken).ConfigureAwait(false)) is not null)
        {
            GlovesParserState newState = state switch
            {
                GlovesParserState.FillNewDto => FindGloveSkinName(line, dto),
                GlovesParserState.LookingForRarity => FindRarity(line, dto),
                GlovesParserState.LookingForImg => FindImg(line, dto),
                _ => state
            };

            if (newState == state)
                continue;

            if (newState == GlovesParserState.CommittingCurrentDto)
            {
                newState = AddGloveToRelations(dto, relations);
                dto = new();
            }

            state = newState;
        }

        return relations.Values.ToList();
    }

    private static GlovesParserState AddGloveToRelations(CurrentDto dto, Dictionary<string, GloveSkinsDto> relations)
    {
        if (dto.GloveName is not null)
        {
            GloveDto skin = dto.Builder.Build();
            if (!relations.TryAdd(dto.GloveName, new() { GloveName = dto.GloveName, Skins = [skin] }))
            {
                GloveSkinsDto gloveSkinsDto = relations[dto.GloveName];
                gloveSkinsDto.Skins.Add(skin);
            }
            return GlovesParserState.FillNewDto;
        }
        return GlovesParserState.FillNewDto;
    }

    private GlovesParserState FindGloveSkinName(string line, CurrentDto dto)
    {
        if (TryGetGloveName(line, out var gloveName) && TryGetSkinName(line, out var skinName))
        {
            dto.GloveName = gloveName;
            dto.Builder.WithSkinName(skinName!);
            return GlovesParserState.LookingForRarity;
        }
        return GlovesParserState.FillNewDto;
    }

    private GlovesParserState FindRarity(string line, CurrentDto dto)
    {
        if (TryGetRarity(line, out var rarity))
        {
            dto.Builder.WithRarity(rarity!);
            return GlovesParserState.LookingForImg;
        }
        return GlovesParserState.LookingForRarity;
    }

    private GlovesParserState FindImg(string line, CurrentDto dto)
    {
        if (TryGetImg(line, out var img))
        {
            dto.Builder.WithImg(img!);
            return GlovesParserState.CommittingCurrentDto;
        }
        return GlovesParserState.LookingForImg;
    }



    private bool TryGetGloveName(string line, out string? gloveName)
    {
        gloveName = ExtractRegex(line, GloveRegex, GlovesParserOptions.GloveNameGroupName);
        return gloveName is not null;
    }

    private bool TryGetSkinName(string line, out string? skinName)
    {
        skinName = ExtractRegex(line, SkinRegex, GlovesParserOptions.SkinGroupName);
        return skinName is not null;
    }

    private bool TryGetRarity(string line, out string? rarity)
    {
        rarity = ExtractRegex(line, RarityRegex, GlovesParserOptions.RarityGroupName);
        return rarity is not null;
    }

    private bool TryGetImg(string line, out string? img)
    {
        img = ExtractRegex(line, ImgRegex, GlovesParserOptions.ImgGroupName);
        return img is not null;
    }
}
