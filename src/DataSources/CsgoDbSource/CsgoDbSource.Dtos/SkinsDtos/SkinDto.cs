using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using CsgoDbSource.Exceptions;

namespace CsgoDbSource.Dtos.SkinsDtos;

public sealed record SkinDto
{
    public required string SkinName { get; init; }
    public required string Rarity { get; init; }
    public required string Img { get; init; }

    [JsonConstructor]
    [SetsRequiredMembers]
    public SkinDto(string skinName, string rarity, string img)
    {
        SkinName = skinName;
        Rarity = rarity;
        Img = img;
    }

    public sealed class Builder
    {
        public string? SkinName { get; private set; }
        public string? Rarity { get; private set; }
        public string? Img { get; private set; }

        public SkinDto Build()
        {
            return new(
                SkinName ?? throw new ArgumentNullException("Argument cannot be null!", nameof(SkinName)),
                Rarity ?? throw new ArgumentNullException("Argument cannot be null!", nameof(Rarity)),
                Img ?? throw new ArgumentNullException("Argument cannot be null!", nameof(Img))
            );
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
            Img = value;
            return this;
        }
    }

}
