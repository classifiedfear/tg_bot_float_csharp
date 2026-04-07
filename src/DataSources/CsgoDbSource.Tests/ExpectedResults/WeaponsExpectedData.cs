using CsgoDbSource.Dtos.WeaponsDtos;

namespace CsgoDbSource.Tests.ExpectedData;

public sealed record WeaponsExpectedData
{
    public int CategoryCount => CategoryNames.Length;
    public int WeaponCount => WeaponCountEachCategory.Sum();
    public string[] CategoryNames { get; init; } = ["Pistols", "Rifles", "SMGs", "Heavy", "Knives", "Others"];
    public int[] WeaponCountEachCategory { get; init; } = [10, 11, 7, 6, 20, 1];
    public WeaponDto[] SampleWeaponPerCategory { get; init; } = [
        MakeWeapon("P2000", "https://www.csgodatabase.com/images/weapons/webp/P2000.webp", 36),
        MakeWeapon("AK-47", "https://www.csgodatabase.com/images/weapons/webp/AK-47.webp",58),
        MakeWeapon("UMP-45", "https://www.csgodatabase.com/images/weapons/webp/UMP-45.webp", 44),
        MakeWeapon("Sawed-Off", "https://www.csgodatabase.com/images/weapons/webp/Sawed-Off.webp", 34),
        MakeWeapon("Shadow Daggers", "https://www.csgodatabase.com/images/knives/webp/Shadow_Daggers.webp", 25),
        MakeWeapon("Zeus x27", "https://www.csgodatabase.com/images/weapons/webp/Zeus_x27.webp", 7)];
    public static WeaponDto MakeWeapon(string name, string imgUrl, int skinCount) =>
        new(name, imgUrl, skinCount);

    public WeaponsPageDto ToPageDto()
    {
        var categoryDtos = new List<CategoryWeaponDto>();

        var categoriesCountsDtos = CategoryNames.Zip(WeaponCountEachCategory, SampleWeaponPerCategory);

        foreach ((string category, int count, WeaponDto dto) in categoriesCountsDtos)
        {
            categoryDtos.Add(
                new()
                {
                    Category = category,
                    WeaponInCategoryCount = count,
                    Weapons = [dto]
                });
        }
        return new(categoryDtos);
    }
}
