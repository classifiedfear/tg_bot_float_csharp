using System;
using System.Diagnostics.CodeAnalysis;

namespace CsgoDbSource.Dtos.GlovesDtos;

[method: SetsRequiredMembers]
public sealed record GlovesPageDto(List<GloveSkinsDto> Gloves)
{
    public List<GloveSkinsDto> Gloves { get; set; } = Gloves;
    public int GloveCount => Gloves.Count;
    public int SkinCount => Gloves.Sum(dto => dto.SkinCount);
}
