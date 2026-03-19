using System;
using CsgoDbSource.Dtos.AgentsDtos;

namespace CsgoDbSource.Dtos.AgentsDtos;

public sealed class AgentSkinsDto
{
    public required string FractionName { get; set; }
    public List<AgentDto> Skins { get; set; } = [];
    public int SkinsCount => Skins.Count;
}
