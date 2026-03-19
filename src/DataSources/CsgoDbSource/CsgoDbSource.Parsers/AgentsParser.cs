using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

using CsgoDbSource.Dtos;
using CsgoDbSource.Dtos.AgentsDtos;
using CsgoDbSource.Parsers.Options;
using CsgoDbSource.Parsers.ParserStates;
using CsgoDbSource.Exceptions;

namespace CsgoDbSource.Parsers;


public class AgentsParser : BaseParser<AgentsPageDto>
{
    private class CurrentDto
    {
        public string? FractionName { get; set; }
        public AgentDto.Builder Builder { get; } = new();
    }

    private Regex FractionRegex { get; init; }
    private Regex SkinRegex { get; init; }
    private Regex RarityRegex { get; init; }
    private Regex ImgRegex { get; init; }
    public AgentsParser(IOptions<AgentsParserOptions> options)
    {
        var _options = options.Value;
        FractionRegex = new(_options.FractionRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        SkinRegex = new(_options.SkinRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        RarityRegex = new(_options.RarityRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        ImgRegex = new(_options.ImgRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);

    }

    public override async Task<AgentsPageDto> GetParsedData(Stream stream, CancellationToken cancellationToken)
    {
        using var streamReader = new StreamReader(stream);

        List<AgentSkinsDto> agentSkinsDtos = await GetAgentSkinsDtos(streamReader, cancellationToken);

        if (agentSkinsDtos.Count == 0)
            throw new SourceStructureException(BaseCsgoDbSourceException.SourceStructureProblem);

        return new() { Agents = agentSkinsDtos, SkinsCount = agentSkinsDtos.Sum(dto => dto.SkinsCount) };
    }

    private async Task<List<AgentSkinsDto>> GetAgentSkinsDtos(StreamReader streamReader, CancellationToken cancellationToken)
    {
        AgentsParserState state = AgentsParserState.FillNewDto;

        Dictionary<string, AgentSkinsDto> relations = [];

        CurrentDto currentDto = new();

        string? line;

        while ((line = await streamReader.ReadLineAsync(cancellationToken).ConfigureAwait(false)) is not null)
        {
            AgentsParserState newState = state switch
            {
                AgentsParserState.FillNewDto => FindAgentSkinName(line, currentDto),
                AgentsParserState.LookingForRarity => FindRarity(line, currentDto),
                AgentsParserState.LookingForImg => FindImg(line, currentDto),
                _ => state
            };

            if (newState == AgentsParserState.CommittingCurrentDto)
                newState = AddSkinToRelations(currentDto, relations);

            if (newState == state)
                continue;

            if (newState == AgentsParserState.CommittingCurrentDto)
            {
                newState = AddSkinToRelations(currentDto, relations);
                currentDto = new();
            }

            state = newState;

        }
        return relations.Values.ToList();
    }

    private static AgentsParserState AddSkinToRelations(CurrentDto currentDto, Dictionary<string, AgentSkinsDto> relations)
    {
        if (currentDto.FractionName is not null)
        {
            AgentDto skin = currentDto.Builder.Build();
            if (!relations.TryAdd(currentDto.FractionName, new() { FractionName = currentDto.FractionName, Skins = [skin] }))
            {
                AgentSkinsDto agentSkinsDto = relations[currentDto.FractionName];
                agentSkinsDto.Skins.Add(skin);
            }
            return AgentsParserState.FillNewDto;
        }
        return AgentsParserState.FillNewDto;
    }

    private AgentsParserState FindImg(string line, CurrentDto currentDto)
    {
        if (TryGetImg(line, out var img))
        {
            currentDto.Builder.WithImg(img!);
            return AgentsParserState.CommittingCurrentDto;
        }
        return AgentsParserState.LookingForImg;
    }

    private AgentsParserState FindRarity(string line, CurrentDto currentDto)
    {
        if (TryGetRarity(line, out var rarity))
        {
            currentDto.Builder.WithRarity(rarity!);
            return AgentsParserState.LookingForImg;
        }
        return AgentsParserState.LookingForRarity;
    }

    private AgentsParserState FindAgentSkinName(string line, CurrentDto currentDto)
    {
        if (TryGetFractionName(line, out var fractionName) && TryGetSkinName(line, out var skinName))
        {
            currentDto.FractionName = fractionName;
            currentDto.Builder.WithSkinName(skinName!);
            return AgentsParserState.LookingForRarity;
        }

        return AgentsParserState.FillNewDto;
    }


    private bool TryGetFractionName(string line, out string? fractionName)
    {
        fractionName = ExtractRegex(line, FractionRegex, AgentsParserOptions.FractionGroupName);

        return fractionName is not null;
    }

    private bool TryGetSkinName(string line, out string? skinName)
    {
        skinName = ExtractRegex(line, SkinRegex, AgentsParserOptions.SkinGroupName);

        return skinName is not null;
    }

    private bool TryGetRarity(string line, out string? rarity)
    {
        rarity = ExtractRegex(line, RarityRegex, AgentsParserOptions.RarityGroupName);

        return rarity is not null;
    }

    private bool TryGetImg(string line, out string? img)
    {
        img = ExtractRegex(line, ImgRegex, AgentsParserOptions.ImgGroupName);

        return img is not null;
    }
}
