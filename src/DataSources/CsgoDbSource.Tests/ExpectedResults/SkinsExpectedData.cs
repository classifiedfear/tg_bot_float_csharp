using System;
using CsgoDbSource.Dtos.SkinsDtos;

namespace CsgoDbSource.Tests.ExpectedData;

public sealed record SkinsExpectedData
{
    public required string WeaponName { get; set; }
    public required List<SkinDto> Skins { get; set; }
    public required int SkinCount { get; set; }
    public static SkinDto MakeSkin(string skin, string imgUrl, string rarity) =>
        new(skin, rarity, imgUrl);

    public SkinsPageDto ToPageDto() => new(WeaponName) { Skins = Skins, SkinCount = SkinCount };
}
