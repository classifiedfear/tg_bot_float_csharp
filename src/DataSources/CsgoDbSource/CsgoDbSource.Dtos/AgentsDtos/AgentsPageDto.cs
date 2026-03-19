using System;

namespace CsgoDbSource.Dtos.AgentsDtos;

public sealed class AgentsPageDto
{
    public List<AgentSkinsDto> Agents { get; set; } = [];
    public int FractionCount => Agents.Count;
    public int SkinsCount { get; set; }
}
