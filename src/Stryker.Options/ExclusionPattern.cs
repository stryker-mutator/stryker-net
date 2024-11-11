using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DotNet.Globbing;
using Stryker.Utilities;

namespace Stryker.Abstractions.Options;

public readonly partial struct ExclusionPattern : IExclusionPattern
{
    public ExclusionPattern(string s)
    {
        if (s is null)
        {
            throw new ArgumentNullException(nameof(s));
        }

        IsExcluded = s.StartsWith('!');

        var pattern = IsExcluded ? s[1..] : s;
        var mutantSpansRegex = MutantSpanGroupRegex().Match(pattern);
        if (mutantSpansRegex.Success)
        {
            var filePathPart = pattern[..^mutantSpansRegex.Length];
            var normalized = FilePathUtils.NormalizePathSeparators(filePathPart);
            Glob = Glob.Parse(normalized);

            MutantSpans = MutantSpanRegex()
                .Matches(mutantSpansRegex.Value)
                .Select(x => (int.Parse(x.Groups[1].Value), int.Parse(x.Groups[2].Value)));
        }
        else
        {
            var normalized = FilePathUtils.NormalizePathSeparators(pattern);
            Glob = Glob.Parse(normalized);
            MutantSpans = Enumerable.Empty<(int, int)>();
        }
    }

    public bool IsExcluded { get; }

    public Glob Glob { get; }

    public IEnumerable<(int Start, int End)> MutantSpans { get; }

    [GeneratedRegex(@"(\{(\d+)\.\.(\d+)\})+$")]
    private static partial Regex MutantSpanGroupRegex();

    [GeneratedRegex(@"\{(\d+)\.\.(\d+)\}")]
    private static partial Regex MutantSpanRegex();
}
