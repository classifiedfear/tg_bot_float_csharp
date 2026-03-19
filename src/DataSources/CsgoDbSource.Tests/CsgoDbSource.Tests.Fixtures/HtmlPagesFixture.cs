using System;

namespace CsgoDbSource.Tests.Fixtures;

public class HtmlPagesFixture
{
    public string WeaponsPage { get; }
    public string GlovesPage { get; }
    public string AgentsPage { get; }
    public string WrongPage { get; }
    private const string testDataDirectory = $"CsgoDbSource.Tests.TestData";

    public HtmlPagesFixture()
    {
        WeaponsPage = File.ReadAllText(GetPathFor("WeaponsPage.txt"));
        GlovesPage = File.ReadAllText(GetPathFor("GlovesPage.txt"));
        AgentsPage = File.ReadAllText(GetPathFor("AgentsPage.txt"));
        WrongPage = File.ReadAllText(GetPathFor("PageWrongStructure.txt"));
    }
    private static string GetPathFor(string file) =>
        Path.Combine(AppContext.BaseDirectory, testDataDirectory, file);

    public string GetSkinPage(string weapon)
    {
        weapon = weapon.Replace(" ", "");
        return File.ReadAllText(GetPathFor(weapon + "SkinPage.txt"));
    }

    public string GetAdditionalInfoPage(string weapon, string skin)
    {
        weapon = weapon.Replace(" ", "").Replace("-", "");
        skin = skin.Replace(" ", "");
        return File.ReadAllText(GetPathFor($"{weapon}{skin}AdditionalInfoPage.txt"));
    }

}

