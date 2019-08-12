using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DotNet.Globbing;
using Microsoft.CodeAnalysis.Text;

namespace Stryker.Core.Options
{
    public class FilePattern
    {
        private static readonly Regex _textSpanGroupRegex = new Regex("(\\{(\\d+)\\.\\.(\\d+)\\})+$");
        private static readonly Regex _textSpanRegex = new Regex("\\{(\\d+)\\.\\.(\\d+)\\}");

        public Glob Glob { get; set; }

        public bool IsExclude { get; set; }

        public IReadOnlyCollection<TextSpan> TextSpans { get; set; }

        public static FilePattern Parse(string pattern)
        {
            var exclude = false;
            IReadOnlyCollection<TextSpan> textSpans = null;

            if (pattern.StartsWith('!'))
            {
                exclude = true;
                pattern = pattern.Substring(1, pattern.Length - 1);
            }

            var textSpanGroupMatch = _textSpanGroupRegex.Match(pattern);
            if (!textSpanGroupMatch.Success)
            {
                // If there are no spans specified, we create one that will cover the whole file.
                textSpans = new[] {new TextSpan(0, int.MaxValue)};
            }
            else
            {
                // If we have one ore more spans we parse them.
                var textSpansMatches = _textSpanRegex.Matches(textSpanGroupMatch.Value);
                textSpans = textSpansMatches
                    .Select(x => TextSpan.FromBounds(int.Parse(x.Groups[1].Value), int.Parse(x.Groups[2].Value))).ToList();

                pattern = pattern.Substring(0, pattern.Length - textSpanGroupMatch.Length);
            }

            var glob = Glob.Parse(pattern);

            return new FilePattern {Glob = glob, IsExclude = exclude, TextSpans = textSpans};
        }
    }
}