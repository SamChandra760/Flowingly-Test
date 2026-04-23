using System.Text.RegularExpressions;
using FlowinglyImport.Api.Common;

namespace FlowinglyImport.Api.Parsing;

public partial class MarkedTextParser
{
    public Result<IReadOnlyList<ParsedTag>> Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var tags = new List<ParsedTag>();
        var errors = new List<ValidationError>();

        foreach (Match match in TagRegex().Matches(text))
        {
            if (match.Groups["closing"].Success)
            {
                continue;
            }

            var tagName = match.Groups["name"].Value;
            var valueStartIndex = match.Index + match.Length;
            var closingTag = $"</{tagName}>";
            var valueEndIndex = text.IndexOf(closingTag, valueStartIndex, StringComparison.OrdinalIgnoreCase);

            if (valueEndIndex < 0)
            {
                errors.Add(new ValidationError("text", $"Opening tag <{tagName}> has no matching closing tag."));
                continue;
            }

            var value = text[valueStartIndex..valueEndIndex].Trim();
            if (!TagRegex().IsMatch(value))
            {
                tags.Add(new ParsedTag(tagName, value));
            }
        }

        return errors.Count > 0
            ? Result<IReadOnlyList<ParsedTag>>.Failure(errors)
            : Result<IReadOnlyList<ParsedTag>>.Success(tags);
    }

    [GeneratedRegex(@"<(?<closing>/)?(?<name>[A-Za-z_][A-Za-z0-9_-]*)\s*>", RegexOptions.Compiled)]
    private static partial Regex TagRegex();
}

public record ParsedTag(string Name, string Value);
