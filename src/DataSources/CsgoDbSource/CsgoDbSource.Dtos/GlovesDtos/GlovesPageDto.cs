using System;

namespace CsgoDbSource.Dtos.GlovesDtos;

public sealed class GlovesPageDto
{
    public List<GloveSkinsDto> Gloves { get; set; } = [];

    public int GlovesCount => Gloves.Count;
    public int SkinsCount { get; set; }
}
