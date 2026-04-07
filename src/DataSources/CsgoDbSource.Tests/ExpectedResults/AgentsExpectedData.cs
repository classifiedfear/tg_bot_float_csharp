using System;
using CsgoDbSource.Dtos.AgentsDtos;

namespace CsgoDbSource.Tests.ExpectedData;

public sealed record AgentsExpectedData
{
    public int AgentCount => AgentNames.Length;
    public int SkinCount => SkinCountEachAgent.Sum();
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
    public AgentDto[] SampleSkinEachAgent { get; init; } = [
        MakeAgent(
                "Chef d'Escadron Rouchard",
                "https://www.csgodatabase.com/images/agents/webp/Chef_d'Escadron_Rouchard_Gendarmerie_Nationale.webp",
                "Master"
            ),
        MakeAgent(
            "'Medium Rare' Crasswater",
            "https://www.csgodatabase.com/images/agents/webp/'Medium_Rare'_Crasswater_Guerrilla_Warfare.webp",
            "Master"
        ),
        MakeAgent(
            "Lieutenant Rex Krikey",
            "https://www.csgodatabase.com/images/agents/webp/Lieutenant_Rex_Krikey_SEAL_Frogman.webp",
            "Superior"
        ),
        MakeAgent(
            "Bloody Darryl The Strapped",
            "https://www.csgodatabase.com/images/agents/webp/Bloody_Darryl_The_Strapped_The_Professionals.webp",
            "Superior"
        ),
        MakeAgent(
            "Cmdr. Mae 'Dead Cold' Jamison",
            "https://www.csgodatabase.com/images/agents/webp/Cmdr._Mae_'Dead_Cold'_Jamison_SWAT.webp",
            "Master"
        ),
        MakeAgent(
            "Primeiro Tenente",
            "https://www.csgodatabase.com/images/agents/webp/Primeiro_Tenente_Brazilian_1st_Battalion.webp",
            "Distinguished"
            ),
        MakeAgent(
            "D Squadron Officer",
            "https://www.csgodatabase.com/images/agents/webp/D_Squadron_Officer_NZSAS.webp",
            "Distinguished"
            ),
        MakeAgent(
            "Mr. Muhlik",
            "https://www.csgodatabase.com/images/agents/webp/Mr._Muhlik_Elite_Crew.webp",
            "Distinguished"
            ),
        MakeAgent(
            "Rezan the Redshirt",
            "https://www.csgodatabase.com/images/agents/webp/Rezan_the_Redshirt_Sabre.webp",
            "Superior"
            ),
        MakeAgent(
            "'Two Times' McCoy",
            "https://www.csgodatabase.com/images/agents/webp/'Two_Times'_McCoy_TACP_Cavalry.webp",
            "Superior"
            ),
        MakeAgent(
            "'Blueberries' Buckshot",
            "https://www.csgodatabase.com/images/agents/webp/'Blueberries'_Buckshot_NSWC_SEAL.webp",
            "Exceptional"
            ),
        MakeAgent(
            "Street Soldier",
            "https://www.csgodatabase.com/images/agents/webp/Street_Soldier_Phoenix.webp",
            "Distinguished"
            ),
        MakeAgent(
            "Dragomir",
            "https://www.csgodatabase.com/images/agents/webp/Dragomir_Sabre_Footsoldier.webp",
            "Distinguished"
            ),
        MakeAgent(
            "Special Agent Ava",
            "https://www.csgodatabase.com/images/agents/webp/Special_Agent_Ava_FBI.webp",
            "Master"
            ),
        MakeAgent(
            "'Two Times' McCoy",
            "https://www.csgodatabase.com/images/agents/webp/'Two_Times'_McCoy_USAF_TACP.webp",
            "Superior"
            ),
        MakeAgent(
            "Michael Syfers",
            "https://www.csgodatabase.com/images/agents/webp/Michael_Syfers_FBI_Sniper.webp",
            "Superior"
            ),
        MakeAgent(
            "Markus Delrow",
            "https://www.csgodatabase.com/images/agents/webp/Markus_Delrow_FBI_HRT.webp",
            "Exceptional"
            ),
        MakeAgent(
            "3rd Commando Company",
            "https://www.csgodatabase.com/images/agents/webp/3rd_Commando_Company_KSK.webp",
            "Distinguished"
            ),
        MakeAgent(
            "B Squadron Officer",
            "https://www.csgodatabase.com/images/agents/webp/B_Squadron_Officer_SAS.webp",
            "Distinguished"
            ),
        MakeAgent(
            "Operator",
            "https://www.csgodatabase.com/images/agents/webp/Operator_FBI_SWAT.webp",
            "Distinguished"
            ),

    ];
    public static AgentDto MakeAgent(string name, string imgUrl, string rarity) =>
        new(name, imgUrl, rarity);

    public AgentsPageDto ToPageDto()
    {
        var agentsSkinDtos = new List<AgentSkinsDto>();

        var namesCountsDtos = AgentNames.Zip(SkinCountEachAgent, SampleSkinEachAgent);

        foreach ((string name, int count, AgentDto dto) in namesCountsDtos)
        {
            agentsSkinDtos.Add(new()
            {
                AgentName = name,
                SkinCount = count,
                Skins = [dto]
            });
        }
        return new(agentsSkinDtos);
    }
}
