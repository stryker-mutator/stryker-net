using System;
using System.Text.RegularExpressions;
using DotNet.Globbing;

namespace Stryker.Core
{
    public readonly struct ExclusionPattern
    {
        private static readonly Regex _mutantSpansRegex = new("(\\{(\\d+)\\.\\.(\\d+)\\})+$");

        public ExclusionPattern(string s)
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            IsExcluded = s.StartsWith('!');

            var pattern = IsExcluded ? s[1..] : s;
            var mutantSpansRegex = _mutantSpansRegex.Match(pattern);
            if (mutantSpansRegex.Success)
            {
                var filePathPart = pattern[..^mutantSpansRegex.Length];
                var normalized = FilePathUtils.NormalizePathSeparators(filePathPart);
                Glob = Glob.Parse(normalized);
                MutantSpans = mutantSpansRegex.Value;
            }
            else
            {
                var normalized = FilePathUtils.NormalizePathSeparators(pattern);
                Glob = Glob.Parse(normalized);
                MutantSpans = string.Empty;
            }
        }

        public bool IsExcluded { get; }

        public Glob Glob { get; }

        public string MutantSpans { get; }
    }
}
