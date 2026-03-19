using CsgoDbSource.Dtos.WeaponsDtos;

namespace CsgoDbSource.Dtos.WeaponsDtos;

public sealed class CategoryWeaponDto
{
    public required string Category { get; init; }
    public List<WeaponDto> Weapons { get; set; } = [];
    public int WeaponInCategoryCount { get; set; }
}
