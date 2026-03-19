using System;

namespace CsgoDbSource.Dtos.GlovesDtos;

public sealed class GloveSkinsDto
{
    public required string GloveName { get; init; }

    public List<GloveDto> Skins { get; set; } = [];

    public int SkinCount => Skins.Count;
}
