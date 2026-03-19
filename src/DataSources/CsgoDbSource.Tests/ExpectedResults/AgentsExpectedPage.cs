using System;
using CsgoDbSource.Dtos.AgentsDtos;

namespace CsgoDbSource.Tests.ExpectedResults;

public sealed record AgentsExpectedPage
{
    public int AgentCount { get; init; } = 20;
    public int SkinCount { get; init; } = 63;
    public string[] AgentNames { get; init; } = [
            "Gendarmerie Nationale", "Guerrilla Warfare", "SEAL Frogman",
            "The Professionals", "SWAT", "Brazilian 1st Battalion",
            "NZSAS", "Elite Crew", "Sabre",
            "TACP Cavalry", "NSWC SEAL", "Phoenix",
            "Sabre Footsoldier", "FBI", "USAF TACP",
            "FBI Sniper", "FBI HRT", "KSK",
            "SAS", "FBI SWAT"
            ];
    public int[] SkinCountEachAgent { get; init; } = [5, 8, 3, 10, 7, 1, 1, 5, 6, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1];
    public AgentDto[] ChosenAgentSkins { get; init; } = [
        MakeAgent(
                "Chef d'Escadron Rouchard",
                "https://www.csgodatabase.com/images/agents/webp/Chef_d'Escadron_Rouchard_Gendarmerie_Nationale.webp",
                "Master"
            ),
        MakeAgent(
            "Aspirant",
            "https://www.csgodatabase.com/images/agents/webp/Aspirant_Gendarmerie_Nationale.webp",
            "Distinguished"
        ),
        MakeAgent(
            "Bloody Darryl The Strapped",
            "https://www.csgodatabase.com/images/agents/webp/Bloody_Darryl_The_Strapped_The_Professionals.webp",
            "Superior"
        ),
        MakeAgent(
            "Safecracker Voltzmann",
            "https://www.csgodatabase.com/images/agents/webp/Safecracker_Voltzmann_The_Professionals.webp",
            "Superior"
        ),
        MakeAgent(
            "Lt. Commander Ricksaw",
            "https://www.csgodatabase.com/images/agents/webp/Lt._Commander_Ricksaw_NSWC_SEAL.webp",
            "Master"
        ),
        MakeAgent(
            "Seal Team 6 Soldier",
            "https://www.csgodatabase.com/images/agents/webp/Seal_Team_6_Soldier_NSWC_SEAL.webp",
            "Distinguished"
                ),
    ];
    public static AgentDto MakeAgent(string name, string url, string rarity) =>
        new AgentDto.Builder()
            .WithSkinName(name)
            .WithImg(url)
            .WithRarity(rarity)
            .Build();
}
