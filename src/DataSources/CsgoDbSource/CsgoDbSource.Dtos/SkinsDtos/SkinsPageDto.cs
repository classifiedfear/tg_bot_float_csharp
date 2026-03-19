using CsgoDbSource.Dtos.SkinsDtos;

namespace CsgoDbSource.Dtos.SkinsDtos;

public sealed class SkinsPageDto
{
    public required string WeaponName { get; init; }
    public List<SkinDto> Skins { get; set; } = [];
    public int SkinCount => Skins.Count;
}