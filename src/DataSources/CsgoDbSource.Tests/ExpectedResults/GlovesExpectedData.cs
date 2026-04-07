using System;
using CsgoDbSource.Dtos.GlovesDtos;

namespace CsgoDbSource.Tests.ExpectedData;

public sealed record GlovesExpectedData
{
    public int GloveCount => GloveNames.Length;
    public int SkinCount => SkinCountEachGlove.Sum();
    public string[] GloveNames { get; init; } = [
        "Bloodhound Gloves",
        "Broken Fang Gloves",
        "Driver Gloves",
        "Hand Wraps",
        "Hydra Gloves",
        "Moto Gloves",
        "Specialist Gloves",
        "Sport Gloves"
    ];
    public int[] SkinCountEachGlove { get; init; } = [4, 4, 12, 12, 4, 12, 12, 12];
    public GloveDto[] SampleSkinEachGlove { get; init; } = [
        MakeGlove("Charred", "https://www.csgodatabase.com/images/gloves/webp/Bloodhound_Gloves_Charred.webp", "Extraordinary"),
        MakeGlove("Jade", "https://www.csgodatabase.com/images/gloves/webp/Broken_Fang_Gloves_Jade.webp", "Extraordinary"),
        MakeGlove("Diamondback", "https://www.csgodatabase.com/images/gloves/webp/Driver_Gloves_Diamondback.webp", "Extraordinary"),
        MakeGlove("CAUTION!", "https://www.csgodatabase.com/images/gloves/webp/Hand_Wraps_CAUTION!.webp", "Extraordinary"),
        MakeGlove("Emerald", "https://www.csgodatabase.com/images/gloves/webp/Hydra_Gloves_Emerald.webp", "Extraordinary"),
        MakeGlove("Polygon", "https://www.csgodatabase.com/images/gloves/webp/Moto_Gloves_Polygon.webp", "Extraordinary"),
        MakeGlove("Buckshot", "https://www.csgodatabase.com/images/gloves/webp/Specialist_Gloves_Buckshot.webp", "Extraordinary"),
        MakeGlove("Arid", "https://www.csgodatabase.com/images/gloves/webp/Sport_Gloves_Arid.webp", "Extraordinary"),
    ];
    public static GloveDto MakeGlove(string name, string imgUrl, string rarity) =>
        new(name, imgUrl, rarity);

    public GlovesPageDto ToPageDto()
    {
        var gloves = new List<GloveSkinsDto>();

        var namesCountsDtos = GloveNames.Zip(SkinCountEachGlove, SampleSkinEachGlove);

        foreach ((string name, int count, GloveDto dto) in namesCountsDtos)
        {
            gloves.Add(new()
            {
                GloveName = name,
                SkinCount = count,
                Skins = [dto]
            });
        }

        return new(gloves);
    }
}