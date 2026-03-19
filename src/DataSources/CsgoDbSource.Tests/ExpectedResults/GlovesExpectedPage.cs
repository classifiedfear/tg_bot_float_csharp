using System;
using CsgoDbSource.Dtos.GlovesDtos;

namespace CsgoDbSource.Tests.ExpectedResults;

public sealed record GlovesExpectedPage
{
    public int GloveCount { get; init; } = 8;
    public int SkinCount { get; init; } = 72;
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
    public GloveDto[] OneSkinEachGlove { get; init; } = [
        MakeGlove("Charred", "https://www.csgodatabase.com/images/gloves/webp/Bloodhound_Gloves_Charred.webp", "Extraordinary"),
        MakeGlove("Jade", "https://www.csgodatabase.com/images/gloves/webp/Broken_Fang_Gloves_Jade.webp", "Extraordinary"),
        MakeGlove("Diamondback", "https://www.csgodatabase.com/images/gloves/webp/Driver_Gloves_Diamondback.webp", "Extraordinary"),
        MakeGlove("CAUTION!", "https://www.csgodatabase.com/images/gloves/webp/Hand_Wraps_CAUTION!.webp", "Extraordinary"),
        MakeGlove("Emerald", "https://www.csgodatabase.com/images/gloves/webp/Hydra_Gloves_Emerald.webp", "Extraordinary"),
        MakeGlove("Polygon", "https://www.csgodatabase.com/images/gloves/webp/Moto_Gloves_Polygon.webp", "Extraordinary"),
        MakeGlove("Buckshot", "https://www.csgodatabase.com/images/gloves/webp/Specialist_Gloves_Buckshot.webp", "Extraordinary"),
MakeGlove("Arid", "https://www.csgodatabase.com/images/gloves/webp/Sport_Gloves_Arid.webp", "Extraordinary"),
    ];
    public static GloveDto MakeGlove(string name, string url, string rarity) =>
        new GloveDto.Builder()
            .WithSkinName(name)
            .WithImg(url)
            .WithRarity(rarity)
            .Build();
}