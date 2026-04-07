using System;

namespace CsgoDbSource.Dtos.GlovesDtos;

public sealed record GloveSkinsDto
{
    public required string GloveName { get; init; }

    public List<GloveDto> Skins { get; set; } = [];

    public int SkinCount { get; set; }
}
