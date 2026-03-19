using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using CsgoDbSource.Exceptions;

namespace CsgoDbSource.Dtos.AdditionalInfoDtos;

public sealed record AdditionalInfoPageDto
{
    public required string WeaponName { get; init; }
    public required string SkinName { get; init; }
    public string? Rarity { get; }
    public string? WeaponSkinImg { get; }
    public ImmutableArray<string> Qualities { get; }
    public ImmutableArray<string> StattrakQualities { get; }
    public bool StattrakExistence { get; }

    [JsonConstructor]
    [SetsRequiredMembers]
    public AdditionalInfoPageDto(
        string weaponName,
        string skinName,
        string? rarity,
        string? weaponSkinImg,
        ImmutableArray<string> qualities,
        ImmutableArray<string> stattrakQualities,
        bool stattrakExistence)
    {
        WeaponName = weaponName;
        SkinName = skinName;
        Rarity = rarity;
        WeaponSkinImg = weaponSkinImg;
        Qualities = qualities;
        StattrakQualities = stattrakQualities;
        StattrakExistence = stattrakExistence;
    }

    public sealed class Builder
    {
        private static readonly string defaultRarity = "Extraordinary";
        public string? WeaponName { get; private set; }
        public string? SkinName { get; private set; }
        public string? Rarity { get; private set; }
        public string? WeaponSkinImg { get; private set; }

        public List<string> Qualities { get; private set; } = [];
        public List<string> StattrakQualities { get; private set; } = [];
        public bool StattrakExistence = false;

        public AdditionalInfoPageDto Build()
        {
            return new AdditionalInfoPageDto(
                WeaponName ?? throw new ArgumentNullException("Argument cannot be null!", nameof(WeaponName)),
                SkinName ?? throw new ArgumentNullException("Argument cannot be null!", nameof(SkinName)),
                Rarity ?? defaultRarity,
                WeaponSkinImg ?? throw new ArgumentNullException("Argument cannot be null!", nameof(WeaponSkinImg)),
                Qualities.ToImmutableArray(),
                StattrakQualities.ToImmutableArray(),
                StattrakExistence
            );
        }

        public Builder WithWeapon(string value)
        {
            WeaponName = value;
            return this;
        }

        public Builder WithSkinName(string value)
        {
            SkinName = value;
            return this;
        }

        public Builder WithRarity(string value)
        {
            Rarity = value;
            return this;
        }

        public Builder WithImg(string value)
        {
            WeaponSkinImg = value;
            return this;
        }

        public Builder WithQualities(List<string> value)
        {
            Qualities = [.. value];
            return this;
        }

        public Builder WithStattrakQualities(List<string> value)
        {
            StattrakQualities = [.. value];
            return this;
        }

        public Builder WithStattrakExistence(bool value)
        {
            StattrakExistence = value;
            return this;
        }
    }
}
