using System;

namespace CsgoDbSource.Dtos.WeaponsDtos;

public sealed class WeaponsPageDto
{
    public List<CategoryWeaponDto> Categories { get; set; } = [];
    public int WeaponCount { get; set; }
    public int CategoryCount => Categories.Count;
}