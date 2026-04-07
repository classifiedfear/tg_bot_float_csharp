using System;
using System.Diagnostics.CodeAnalysis;

namespace CsgoDbSource.Dtos.AgentsDtos;

[method: SetsRequiredMembers]
public sealed record AgentsPageDto(List<AgentSkinsDto> Agents)
{
    public List<AgentSkinsDto> Agents { get; set; } = Agents;
    public int AgentCount => Agents.Count;
    public int SkinCount => Agents.Sum(dto => dto.SkinCount);
}
