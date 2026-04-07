using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;

using CsgoDbSource.Dtos;
using CsgoDbSource.Parsers.Options;
using CsgoDbSource.Parsers.ParserStates;
using CsgoDbSource.Exceptions;
using CsgoDbSource.Dtos.WeaponsDtos;


namespace CsgoDbSource.Parsers;


public sealed class WeaponsParser : BaseParser<WeaponsPageDto>
{
    private readonly string othersCategory = "Others";
    private Regex TotalWeaponRegex { get; init; }
    private Regex NameRegex { get; init; }
    private Regex CategoryCountRegex { get; init; }
    private Regex TagsRegex { get; init; }
    private Regex ImgRegex { get; init; }
    private Regex TotalSkinRegex { get; init; }

    public WeaponsParser(IOptions<WeaponsParserOptions> options)
    {
        var _options = options.Value;
        TotalWeaponRegex = new(_options.TotalWeaponRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        NameRegex = new(_options.NameRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        CategoryCountRegex = new(_options.CategoryCountRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        TagsRegex = new(_options.TagsRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        ImgRegex = new(_options.ImgRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        TotalSkinRegex = new(_options.TotalSkinsRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
    }

    public override async Task<WeaponsPageDto> GetParsedData(Stream stream, CancellationToken cancellationToken)
    {
        using var streamReader = new StreamReader(stream);


        var categoryWeaponDtos = new List<CategoryWeaponDto>();

        await foreach (var dto in GetCategoryWeaponDtos(streamReader, cancellationToken))
        {
            if (dto.WeaponInCategoryCount == 0)
                throw new SourceStructureException(BaseCsgoDbSourceException.SourceStructureProblem);

            categoryWeaponDtos.Add(dto);
        }

        WeaponsPageDto page = new(categoryWeaponDtos);

        if (page.CategoryCount == 0)
            throw new SourceStructureException(BaseCsgoDbSourceException.SourceStructureProblem);

        return page;
    }

    private async IAsyncEnumerable<CategoryWeaponDto> GetCategoryWeaponDtos(StreamReader streamReader, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Queue<CategoryWeaponDto> categoryWeaponDtos = new();

        WeaponsParserState state = WeaponsParserState.LookingTotalWeapon;
        WeaponDto.Builder? builder = null;

        string? line;
        while ((line = await streamReader.ReadLineAsync(cancellationToken).ConfigureAwait(false)) is not null)
        {
            WeaponsParserState newState = state switch
            {
                WeaponsParserState.LookingTotalWeapon => FillCategoryDtosWithoutNames(line, categoryWeaponDtos),
                WeaponsParserState.LookingWeaponName => FindWeaponName(line, builder!),
                WeaponsParserState.LookingWeaponImg => FindImg(line, builder!),
                WeaponsParserState.LookingTotalSkins => FindTotalSkins(line, builder!),
                _ => state
            };

            if (newState == state)
                continue;

            if (newState == WeaponsParserState.AddWeaponToCategory)
            {
                CategoryWeaponDto categoryDto = categoryWeaponDtos.Peek();
                categoryDto.Weapons.Add(builder!.Build());

                if (!categoryDto.Category.Equals(othersCategory) && IsCategoryDtoFull(categoryDto))
                    yield return categoryWeaponDtos.Dequeue();

                newState = WeaponsParserState.LookingWeaponName;
            }

            state = newState;

            if (state == WeaponsParserState.LookingWeaponName)
                builder = new WeaponDto.Builder();
        }


        while (categoryWeaponDtos.Count > 0)
        {
            CategoryWeaponDto others = categoryWeaponDtos.Dequeue();
            others.WeaponInCategoryCount = others.Weapons.Count;
            yield return others;
        }

    }

    private WeaponsParserState FillCategoryDtosWithoutNames(string line, Queue<CategoryWeaponDto> categoryWeaponDtos)
    {
        if (TryGetTotalWeapon(line, out var totalWeapon))
        {
            foreach (var dto in GetCategoryDtosWithoutNames(totalWeapon!))
            {
                categoryWeaponDtos.Enqueue(dto);
            }
            return WeaponsParserState.LookingWeaponName;
        }
        return WeaponsParserState.LookingTotalWeapon;

    }

    private WeaponsParserState FindTotalSkins(string line, WeaponDto.Builder builder)
    {
        if (TryGetTotalSkins(line, out var totalSkins))
        {
            if (!int.TryParse(totalSkins, out var result))
            {
                throw new SourceStructureException(BaseCsgoDbSourceException.SourceStructureProblem);
            }
            builder.WithTotalSkinCount(result);
            return WeaponsParserState.AddWeaponToCategory;
        }
        return WeaponsParserState.LookingTotalSkins;
    }

    private WeaponsParserState FindImg(string line, WeaponDto.Builder builder)
    {
        if (TryGetImg(line, out var img))
        {
            builder.WithImg(img!);
            return WeaponsParserState.LookingTotalSkins;
        }
        return WeaponsParserState.LookingWeaponImg;
    }

    private WeaponsParserState FindWeaponName(string line, WeaponDto.Builder builder)
    {
        if (TryGetWeaponName(line, out var weaponName))
        {
            builder.WithWeaponName(weaponName!);
            return WeaponsParserState.LookingWeaponImg;
        }
        return WeaponsParserState.LookingWeaponName;
    }

    private bool TryGetTotalWeapon(string line, out string? totalWeapon)
    {
        totalWeapon = ExtractRegex(line, TotalWeaponRegex, WeaponsParserOptions.TotalWeaponGroupName);

        return totalWeapon is not null;
    }

    private static bool IsCategoryDtoFull(CategoryWeaponDto currentDto) => currentDto.WeaponInCategoryCount == currentDto.Weapons.Count;


    private bool TryGetWeaponName(string line, out string? weaponName)
    {
        weaponName = ExtractRegex(line, NameRegex, WeaponsParserOptions.WeaponGroupName);
        return weaponName is not null;
    }

    private bool TryGetImg(string line, out string? img)
    {
        img = ExtractRegex(line, ImgRegex, WeaponsParserOptions.ImgGroupName);
        return img is not null;
    }

    private bool TryGetTotalSkins(string line, out string? totalSkins)
    {
        totalSkins = ExtractRegex(line, TotalSkinRegex, WeaponsParserOptions.TotalSkinsGroupName);
        return totalSkins is not null;
    }

    private IEnumerable<CategoryWeaponDto> GetCategoryDtosWithoutNames(string line)
    {
        string textWithoutTags = TagsRegex.Replace(line, "");

        var categoryNumberItems = CategoryCountRegex.Matches(textWithoutTags);

        foreach (Match match in categoryNumberItems)
        {
            if (
                match.Groups.TryGetValue(WeaponsParserOptions.CategoryGroupName, out Group? categoryGroup)
                    && match.Groups.TryGetValue(WeaponsParserOptions.CountGroupName, out Group? numberGroup)
                )
            {
                string category = categoryGroup.Value == "heavy weapons" ? "Heavy" : char.ToUpperInvariant(categoryGroup.Value[0]) + categoryGroup.Value[1..];

                if (!int.TryParse(numberGroup.Value, out var count))
                {
                    throw new SourceStructureException(BaseCsgoDbSourceException.SourceStructureProblem);
                }

                yield return new() { Category = category, WeaponInCategoryCount = count };
            }
        }

        yield return new() { Category = othersCategory, WeaponInCategoryCount = 0 };
    }
}