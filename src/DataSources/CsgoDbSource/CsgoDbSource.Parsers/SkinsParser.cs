using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

using CsgoDbSource.Dtos.SkinsDtos;
using CsgoDbSource.Parsers.Options;
using CsgoDbSource.Parsers.ParserStates;
using CsgoDbSource.Exceptions;

namespace CsgoDbSource.Parsers;

public sealed class SkinsParser : BaseParser<SkinsPageDto>
{
    private Regex NameRegex { get; init; }
    private Regex WeaponNameRegex { get; init; }
    private Regex ImgRegex { get; init; }
    private Regex RarityRegex { get; init; }

    public SkinsParser(IOptions<SkinsParserOptions> options)
    {
        var _options = options.Value;
        NameRegex = new(_options.NameRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        WeaponNameRegex = new(_options.WeaponNameRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        RarityRegex = new(_options.RarityRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        ImgRegex = new(_options.ImgRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
    }

    public override async Task<SkinsPageDto> GetParsedData(
        Stream stream, CancellationToken cancellationToken
    )
    {
        using var streamReader = new StreamReader(stream);
        SkinsPageDto pageDto = await GetSkinsPageDto(streamReader, cancellationToken);

        if (pageDto.SkinCount == 0)
            throw new SourceStructureException(BaseCsgoDbSourceException.SourceStructureProblem);

        return pageDto;
    }

    private async Task<SkinsPageDto> GetSkinsPageDto(StreamReader streamReader, CancellationToken cancellationToken)
    {
        SkinsPageDto? pageDto = null;

        SkinsParserState state = SkinsParserState.LookingForWeaponName;
        SkinDto.Builder? builder = null;

        string? line;
        while ((
            line = await streamReader.ReadLineAsync(cancellationToken)
                .ConfigureAwait(false)) is not null
            )
        {

            SkinsParserState newState = state switch
            {
                SkinsParserState.LookingForSkins => FindSkinName(line, builder!),
                SkinsParserState.LookingForRarity => FindRarity(line, builder!),
                SkinsParserState.LookingForImg => FindImg(line, builder!),
                _ => state
            };

            if (
                newState == SkinsParserState.LookingForWeaponName &&
                    TryGetWeaponName(line, out var weaponName)
                )
            {
                pageDto = new() { WeaponName = weaponName! };
                newState = SkinsParserState.PrepToNewSkin;
            }

            if (newState == state)
                continue;

            if (newState == SkinsParserState.CommitSkin)
            {
                pageDto!.Skins.Add(builder!.Build());
                newState = SkinsParserState.PrepToNewSkin;
            }

            if (newState == SkinsParserState.PrepToNewSkin)
            {
                builder = new();
                newState = SkinsParserState.LookingForSkins;
            }

            state = newState;

        }
        return pageDto ?? throw new PageException(BaseCsgoDbSourceException.NotFound);
    }

    private SkinsParserState FindSkinName(string line, SkinDto.Builder builder)
    {
        if (TryGetSkinName(line, out var skinName))
        {
            builder.WithSkinName(skinName!);
            return SkinsParserState.LookingForRarity;
        }
        return SkinsParserState.LookingForSkins;
    }

    private SkinsParserState FindRarity(string line, SkinDto.Builder builder)
    {
        if (TryGetRarity(line, out var rarityName))
        {
            builder.WithRarity(rarityName!);
            return SkinsParserState.LookingForImg;
        }
        return SkinsParserState.LookingForRarity;
    }

    private SkinsParserState FindImg(string line, SkinDto.Builder builder)
    {
        if (TryGetImg(line, out var img))
        {
            builder.WithImg(img!);
            return SkinsParserState.CommitSkin;
        }
        return SkinsParserState.LookingForImg;
    }


    private bool TryGetRarity(string line, out string? rarityName)
    {
        rarityName = ExtractRegex(line, RarityRegex, SkinsParserOptions.RarityGroupName);
        return rarityName is not null;
    }

    private bool TryGetSkinName(string line, out string? skinName)
    {
        skinName = ExtractRegex(line, NameRegex, SkinsParserOptions.SkinGroupName);
        return skinName is not null;
    }

    private bool TryGetWeaponName(string line, out string? weaponName)
    {
        weaponName = ExtractRegex(line, WeaponNameRegex, SkinsParserOptions.WeaponGroupName);
        return weaponName is not null;
    }

    private bool TryGetImg(string line, out string? img)
    {
        img = ExtractRegex(line, ImgRegex, SkinsParserOptions.ImgGroupName);
        return img is not null;
    }
}
