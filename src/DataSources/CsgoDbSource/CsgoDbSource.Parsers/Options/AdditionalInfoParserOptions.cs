using System;
using System.ComponentModel.DataAnnotations;

namespace CsgoDbSource.Parsers.Options;

public sealed class AdditionalInfoParserOptions
{
    internal const string WeaponSkinNameGroupName = "WeaponSkinName";
    internal const string ImgGroupName = "Img";
    internal const string StattrakGroupName = "Stattrak";
    internal const string QualityGroupName = "Quality";
    internal const string RarityGroupName = "Rarity";

    [Required]
    [RegularExpression($"\\(\\?<{StattrakGroupName}>.*\\)\\(\\?<{QualityGroupName}>.*\\')")]
    public required string QualityStattrakRegex { get; init; }

    [Required]
    [RegularExpression($"\\(\\?<{RarityGroupName}>.*\\)")]
    public required string RarityRegex { get; init; }

    [Required]
    [RegularExpression($"\\(\\?<{ImgGroupName}>.*\\)")]
    public required string ImgRegex { get; init; }

    [Required]
    [RegularExpression($"\\(\\?<{WeaponSkinNameGroupName}>.*\\)")]
    public required string AdditionalWeaponSkinNameRegex { get; init; }
}