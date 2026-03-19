using System.ComponentModel.DataAnnotations;

namespace CsgoDbSource.Parsers.Options;

public sealed class WeaponsParserOptions
{
    internal const string TotalWeaponGroupName = "TotalText";
    internal const string WeaponGroupName = "Name";

    internal const string CategoryGroupName = "Category";

    internal const string CountGroupName = "Count";
    internal const string ImgGroupName = "Img";
    internal const string TotalSkinsGroupName = "TotalSkins";

    [Required]
    [RegularExpression($"\\(\\?<{TotalWeaponGroupName}>.*\\)")]
    public required string TotalWeaponRegex { get; init; }

    [Required]
    [RegularExpression($"\\(\\?<{WeaponGroupName}>.*\\)")]
    public required string NameRegex { get; init; }

    [Required]
    [RegularExpression($"\\(\\?<{CountGroupName}>.*\\)\\(\\?<{CategoryGroupName}>.*\\)")]
    public required string CategoryCountRegex { get; init; }

    [Required]
    [RegularExpression($"<[^>]+>")]
    public required string TagsRegex { get; init; }

    [Required]
    [RegularExpression($"\\(?<{ImgGroupName}>.*\\)")]
    public required string ImgRegex { get; init; }

    [Required]
    [RegularExpression($"\\(?<{TotalSkinsGroupName}>.*\\)")]
    public required string TotalSkinsRegex { get; init; }
}