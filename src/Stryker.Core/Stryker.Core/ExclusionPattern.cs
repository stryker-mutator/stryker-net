using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DotNet.Globbing;

namespace Stryker.Core
{
    public readonly struct ExclusionPattern
    {
        private static readonly Regex _mutantSpanGroupRegex = new("(\\{(\\d+)\\.\\.(\\d+)\\})+$");
        private static readonly Regex _mutantSpanRegex = new Regex("\\{(\\d+)\\.\\.(\\d+)\\}");

        public ExclusionPattern(string s)
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            IsExcluded = s.StartsWith('!');

            var pattern = IsExcluded ? s[1..] : s;
            var mutantSpansRegex = _mutantSpanGroupRegex.Match(pattern);
            if (mutantSpansRegex.Success)
            {
                var filePathPart = pattern[..^mutantSpansRegex.Length];
                var normalized = FilePathUtils.NormalizePathSeparators(filePathPart);
                Glob = Glob.Parse(normalized);

                MutantSpans = _mutantSpanRegex
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
    }
}
