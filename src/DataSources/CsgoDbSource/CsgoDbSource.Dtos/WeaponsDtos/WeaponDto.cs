using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using CsgoDbSource.Exceptions;

namespace CsgoDbSource.Dtos.WeaponsDtos;

public sealed record WeaponDto
{
    public required string WeaponName { get; init; }
    public required string Img { get; init; }
    public required int TotalSkinCount { get; init; }

    [JsonConstructor]
    [SetsRequiredMembers]
    public WeaponDto(string weaponName, string img, int totalSkinCount)
    {
        WeaponName = weaponName;
        Img = img;
        TotalSkinCount = totalSkinCount;
    }

    public sealed class Builder
    {
        public string? WeaponName { get; private set; }
        public string? Img { get; private set; }
        public int TotalSkinCount { get; private set; }

        public WeaponDto Build()
        {
            return new(
                WeaponName ?? throw new ArgumentNullException("Argument cannot be null!", nameof(WeaponName)),
                Img ?? throw new ArgumentNullException("Argument cannot be null!", nameof(Img)),
                TotalSkinCount == 0 ? throw new ArgumentException("Argument must be more than 0", nameof(TotalSkinCount)) : TotalSkinCount
                );
        }

        public Builder WithWeaponName(string value)
        {
            WeaponName = value;
            return this;
        }

        public Builder WithImg(string value)
        {
            Img = value;
            return this;
        }

        public Builder WithTotalSkinCount(int value)
        {
            TotalSkinCount = value;
            return this;
        }
    }
}
