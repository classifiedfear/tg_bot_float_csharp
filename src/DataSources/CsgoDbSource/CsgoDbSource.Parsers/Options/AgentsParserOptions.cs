using System.ComponentModel.DataAnnotations;

namespace CsgoDbSource.Parsers.Options;

public class AgentsParserOptions
{
    internal const string FractionGroupName = "FractionName";
    internal const string SkinGroupName = "SkinName";
    internal const string RarityGroupName = "Rarity";
    internal const string ImgGroupName = "Img";

    [Required]
    [RegularExpression($"\\(\\?<{FractionGroupName}>.*\\)")]
    public required string FractionRegex { get; init; }

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
