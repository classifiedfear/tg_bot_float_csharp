using System;
using System.Diagnostics.CodeAnalysis;

namespace CsgoDbSource.Dtos.WeaponsDtos;

[method: SetsRequiredMembers]
public sealed record WeaponsPageDto(List<CategoryWeaponDto> Categories)
{
    public required List<CategoryWeaponDto> Categories { get; init; } = Categories;
    public int WeaponCount => Categories.Sum(dto => dto.WeaponInCategoryCount);
    public int CategoryCount => Categories.Count;
}