using System;
using System.ComponentModel.DataAnnotations;

namespace CsgoDbSource.Parsers.Options;

public sealed class GlovesParserOptions
{
    internal const string GloveNameGroupName = "GloveName";
    internal const string SkinGroupName = "SkinName";
    internal const string RarityGroupName = "Rarity";
    internal const string ImgGroupName = "Img";

    [Required]
    [RegularExpression($"\\(\\?<{GloveNameGroupName}>.*\\)")]
    public required string GloveRegex { get; init; }

    [Required]
    [RegularExpression($"\\(\\?<{SkinGroupName}>.*\\)")]
    public required string SkinRegex { get; init; }

    [Required]
    [RegularExpression($"\\(\\?<{RarityGroupName}>.*\\)")]
    public required string RarityRegex { get; init; }

    [Required]
    [RegularExpression($"\\(\\?<{ImgGroupName}>.*\\)")]
    public required string ImgRegex { get; init; }


}
