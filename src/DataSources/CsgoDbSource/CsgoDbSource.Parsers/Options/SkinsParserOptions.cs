using System;
using System.ComponentModel.DataAnnotations;

namespace CsgoDbSource.Parsers.Options;

public sealed class SkinsParserOptions
{
    internal const string WeaponGroupName = "WeaponName";
    internal const string SkinGroupName = "SkinName";
    internal const string RarityGroupName = "Rarity";
    internal const string ImgGroupName = "Img";

    [Required]
    [RegularExpression($"\\(\\?<{SkinGroupName}>.*\\)")]
    public required string NameRegex { get; init; }
    [Required]
    [RegularExpression($"\\(\\?<{WeaponGroupName}>.*\\)")]
    public required string WeaponNameRegex { get; init; }
    [Required]
    [RegularExpression($"\\(\\?<{RarityGroupName}>.*\\)")]
    public required string RarityRegex { get; init; }
    [Required]
    [RegularExpression($"\\(\\?<{ImgGroupName}>.*\\)")]
    public required string ImgRegex { get; init; }
}
