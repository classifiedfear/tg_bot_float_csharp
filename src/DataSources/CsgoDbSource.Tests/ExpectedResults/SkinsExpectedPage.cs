using System;
using CsgoDbSource.Dtos.SkinsDtos;

namespace CsgoDbSource.Tests.ExpectedResults;

public sealed record SkinsExpectedPage
{
    public required string WeaponName { get; set; }
    public required List<SkinDto> Skins { get; set; }
    public required int SkinCount { get; set; }
    public static SkinDto MakeSkin(string skin, string img, string rarity) =>
        new SkinDto.Builder()
            .WithSkinName(skin)
            .WithImg(img)
            .WithRarity(rarity).Build();
}
