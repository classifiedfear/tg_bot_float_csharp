using System.Diagnostics.CodeAnalysis;
using CsgoDbSource.Dtos.SkinsDtos;

namespace CsgoDbSource.Dtos.SkinsDtos;

[method: SetsRequiredMembers]
public sealed record SkinsPageDto(string WeaponName)
{
    public required string WeaponName { get; init; } = WeaponName;
    public List<SkinDto> Skins { get; set; } = [];
    public int SkinCount { get; set; }
}