using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

using CsgoDbSource.Dtos;
using CsgoDbSource.Parsers.Options;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Dtos.AdditionalInfoDtos;

namespace CsgoDbSource.Parsers;


public class AdditionalInfoParser : BaseParser<AdditionalInfoPageDto>
{
    private sealed class QualityStattrakDto
    {
        public List<string> Qualities { get; set; } = [];
        public List<string> StattrakQualities { get; set; } = [];
        public bool StattrakExistence { get; set; } = false;
    }
    private Regex QualityStattrakRegex { get; init; }
    private Regex RarityRegex { get; init; }
    private Regex AdditionalWeaponSkinNameRegex { get; init; }
    private Regex ImgRegex { get; init; }
    public AdditionalInfoParser(IOptions<AdditionalInfoParserOptions> options)
    {
        var _options = options.Value;
        QualityStattrakRegex = new(_options.QualityStattrakRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        AdditionalWeaponSkinNameRegex = new(_options.AdditionalWeaponSkinNameRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        RarityRegex = new(_options.RarityRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        ImgRegex = new(_options.ImgRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
    }

    public override async Task<AdditionalInfoPageDto> GetParsedData(Stream stream, CancellationToken cancellationToken)
    {
        using var streamReader = new StreamReader(stream);
        AdditionalInfoPageDto pageDto = await GetPage(streamReader, cancellationToken);
        return pageDto;
    }

    private async Task<AdditionalInfoPageDto> GetPage(StreamReader streamReader, CancellationToken cancellationToken)
    {
        AdditionalInfoPageDto.Builder builder = new();
        string? line;

        while ((line = await streamReader.ReadLineAsync(cancellationToken).ConfigureAwait(false)) is not null)
        {
            if (line.Contains("Error 404: Page not found"))
                throw new PageException(BaseCsgoDbSourceException.NotFound);

            if (!IsWeaponSkinNameFinded(builder) && TryGetWeaponSkinName(line, out var weaponSkinName))
            {
                {
                    (string weaponName, string skinName) = GetCorrectWeaponSkinName(weaponSkinName!);
                    builder.WithWeapon(weaponName).WithSkinName(skinName);
                }
            }

            if (!IsImgFinded(builder) && TryGetImg(line, out var img))
                builder.WithImg(img!);

            if (!IsRarityFinded(builder) && TryGetRarity(line, out var rarity))
                builder.WithRarity(rarity!);

            if (!IsQualitiesFinded(builder) && TryGetQualityStattrakDto(line, out var qualityStattrakDto))
                builder.WithQualities(qualityStattrakDto.Qualities)
                    .WithStattrakQualities(qualityStattrakDto.StattrakQualities)
                    .WithStattrakExistence(qualityStattrakDto.StattrakExistence);

            if (PageIsFull(builder))
                break;
        }
        try
        {
            return builder.Build();
        }
        catch (Exception exc)
        {
            if (exc is ArgumentException || exc is ArgumentNullException)
                throw new PageException(BaseCsgoDbSourceException.NotFound);
            else
                throw;
        }
    }

    private static bool IsWeaponSkinNameFinded(AdditionalInfoPageDto.Builder builder) => builder.WeaponName is not null &&
        builder.SkinName is not null;

    private static bool IsRarityFinded(AdditionalInfoPageDto.Builder builder) => builder.Rarity is not null;

    private static bool IsImgFinded(AdditionalInfoPageDto.Builder builder) => builder.WeaponSkinImg is not null;

    private static bool IsQualitiesFinded(AdditionalInfoPageDto.Builder builder) => builder.Qualities.Count != 0;

    private static bool PageIsFull(AdditionalInfoPageDto.Builder builder) => IsWeaponSkinNameFinded(builder) &&
        IsRarityFinded(builder) &&
        IsImgFinded(builder) &&
        IsQualitiesFinded(builder);

    private static (string weaponName, string skinName) GetCorrectWeaponSkinName(string weaponSkinName)
    {
        try
        {
            int separatorIndex = weaponSkinName!.IndexOf('|');
            string weaponName = weaponSkinName[..separatorIndex].Trim();
            string skinName = weaponSkinName[(separatorIndex + 1)..].Trim();
            return (weaponName, skinName);

        }
        catch (ArgumentOutOfRangeException)
        {
            throw new SourceStructureException(BaseCsgoDbSourceException.SourceStructureProblem);
        }
    }


    private bool TryGetWeaponSkinName(string line, out string? weaponSkinName)
    {
        weaponSkinName = ExtractRegex(line, AdditionalWeaponSkinNameRegex, AdditionalInfoParserOptions.WeaponSkinNameGroupName);
        return weaponSkinName is not null;
    }

    private bool TryGetImg(string line, out string? img)
    {
        img = ExtractRegex(line, ImgRegex, AdditionalInfoParserOptions.ImgGroupName);

        return img is not null;
    }

    private bool TryGetRarity(string line, out string? rarity)
    {
        rarity = ExtractRegex(line, RarityRegex, AdditionalInfoParserOptions.RarityGroupName);

        return rarity is not null;
    }

    private bool TryGetQualityStattrakDto(string line, out QualityStattrakDto dto)
    {
        dto = new();

        foreach (Match match in QualityStattrakRegex.Matches(line))
        {
            if (!match.Success) continue;

            if (
                match.Groups.TryGetValue(AdditionalInfoParserOptions.StattrakGroupName, out Group? stattrakGroup) &&
                    stattrakGroup.Success
                )
            {
                dto.StattrakQualities.Add(stattrakGroup.Value);
                dto.StattrakExistence = true;

            }
            else if (
                match.Groups.TryGetValue(AdditionalInfoParserOptions.QualityGroupName, out Group? qualityGroup) &&
                    qualityGroup.Success
            )
                dto.Qualities.Add(qualityGroup.Value);
        }

        return dto.Qualities.Count != 0;
    }
}
