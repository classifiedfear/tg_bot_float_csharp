namespace CsgoDbSource.Parsers;

using System.Text.RegularExpressions;
public abstract class BaseParser<T>
{
    abstract public Task<T> GetParsedData(Stream stream, CancellationToken cancellationToken);

    protected static string? ExtractRegex(string line, Regex pattern, string groupName)
    => pattern.Match(line) is Match match &&
        match.Success &&
            match.Groups.TryGetValue(groupName, out Group? group) ? group.Value : null;

    protected static Match? GetMatch(string line, Regex pattern)
    => pattern.Match(line) is Match match &&
        match.Success ? match : null;

    protected virtual IEnumerable<string> GetIterInfo(string line, Regex pattern, string groupName)
    {
        foreach (Match match in pattern.Matches(line))
        {
            if (match.Success && match.Groups.TryGetValue(groupName, out Group? group))
                yield return group.Value;
        }
    }
}