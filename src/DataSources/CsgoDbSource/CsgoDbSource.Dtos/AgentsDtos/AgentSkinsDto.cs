using System;
using CsgoDbSource.Dtos.AgentsDtos;

namespace CsgoDbSource.Dtos.AgentsDtos;

public sealed record AgentSkinsDto
{
    public required string AgentName { get; set; }
    public List<AgentDto> Skins { get; set; } = [];
    public int SkinCount { get; set; }
}
