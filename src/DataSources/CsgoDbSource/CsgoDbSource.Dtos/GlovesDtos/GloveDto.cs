using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using CsgoDbSource.Exceptions;

namespace CsgoDbSource.Dtos.GlovesDtos;

public sealed record GloveDto
{
    public required string SkinName { get; init; }

    public required string Img { get; init; }

    public required string Rarity { get; init; }

    [JsonConstructor]
    [SetsRequiredMembers]
    public GloveDto(string skinName, string img, string rarity)
    {
        SkinName = skinName;
        Img = img;
        Rarity = rarity;
    }

    public sealed class Builder()
    {
        public string? SkinName { get; private set; }
        public string? Img { get; private set; }
        public string? Rarity { get; private set; }

        public GloveDto Build()
        {
            return new(
                SkinName ?? throw new ArgumentNullException("Argument cannot be null!", nameof(SkinName)),
                Img ?? throw new ArgumentNullException("Argument cannot be null!", nameof(Img)),
                Rarity ?? throw new ArgumentNullException("Argument cannot be null!", nameof(Rarity))
            );
        }

        public Builder WithSkinName(string value)
        {
            SkinName = value;
            return this;
        }

        public Builder WithImg(string value)
        {
            Img = value;
            return this;
        }

        public Builder WithRarity(string value)
        {
            Rarity = value;
            return this;
        }
    }
}
