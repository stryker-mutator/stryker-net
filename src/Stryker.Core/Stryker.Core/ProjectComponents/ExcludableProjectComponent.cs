using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DotNet.Globbing;

namespace Stryker.Core.ProjectComponents
{
    public abstract class ExcludableProjectComponent<T, TSpan> : ProjectComponent<T>
    {
        private static readonly Regex _mutantSpanGroupRegex = new Regex("(\\{(\\d+)\\.\\.(\\d+)\\})+$");
        private static readonly Regex _mutantSpanRegex = new Regex("\\{(\\d+)\\.\\.(\\d+)\\}");
        private static readonly MutantSpan _mutantSpanMaxValue = new(0, int.MaxValue);

        public IEnumerable<FilePattern> Patterns { get; } = Enumerable.Empty<FilePattern>();

        public ExcludableProjectComponent(IEnumerable<ExcludableString> strings)
        {
            Patterns = strings.Select(Parse);
        }

        protected ExcludableProjectComponent()
        {
            Patterns = (new[] { new ExcludableString("**/*") }).Select(Parse);
        }

        public bool IsMatch(MutantSpan span)
        {
            var includePatterns = Patterns.Where(x => !x.IsExclude);
            var excludePatterns = Patterns.Where(x => x.IsExclude);

            if (!includePatterns.Any(MatchesPattern))
            {
                return false;
            }

            return !excludePatterns.Any(MatchesPattern);

            bool MatchesPattern(FilePattern pattern)
            {
                // We check both the full and the relative path to allow for relative paths.
                return IsMatch(pattern, FullPath, span) ||
                       IsMatch(pattern, RelativePath, span);
            }
        }

        public bool IsComponentExcluded()
        {
            var includePattern = Patterns.Where(x => !x.IsExclude).ToList();
            var excludePattern = Patterns.Where(x => x.IsExclude).ToList();

            // Get in- and excluded spans
            var includedSpans = Reduce(includePattern.Where(MatchesFilePattern).SelectMany(x => x.MutantSpans).Select(FromMutantSpan));
            var excludedSpans = Reduce(excludePattern.Where(MatchesFilePattern).SelectMany(x => x.MutantSpans).Select(FromMutantSpan));

            // If there are only included spans, the file is not excluded.
            if ((includedSpans.Any() && !excludedSpans.Any()) || Parent?.Parent == null)
            {
                return false;
            }

            return !RemoveOverlap(includedSpans, excludedSpans).Any();

            bool MatchesFilePattern(FilePattern pattern) =>
                pattern.Glob.IsMatch(FullPath) ||
                pattern.Glob.IsMatch(RelativePath);
        }

        internal FilePattern Parse(ExcludableString s)
        {
            IReadOnlyCollection<MutantSpan> mutantSpans;

            var pattern = s.Pattern;
            var mutantSpanGroupMatch = _mutantSpanGroupRegex.Match(s.Pattern);
            if (!mutantSpanGroupMatch.Success)
            {
                // If there are no spans specified, we add one that will cover the whole file.
                mutantSpans = new[] { _mutantSpanMaxValue };
            }
            else
            {
                // If we have one or more spans we parse them.
                var mutantSpanMatches = _mutantSpanRegex.Matches(mutantSpanGroupMatch.Value);
                var allMutantSpans = mutantSpanMatches
                    .Select(x => new MutantSpan(int.Parse(x.Groups[1].Value), int.Parse(x.Groups[2].Value)))
                    .Select(FromMutantSpan);

                mutantSpans =
                    Reduce(allMutantSpans).Select(ToMutantSpan).ToList();

                pattern = pattern.Substring(0, pattern.Length - mutantSpanGroupMatch.Length);
            }

            var glob = Glob.Parse(FilePathUtils.NormalizePathSeparators(pattern));

            return new FilePattern(glob, s.IsExclude, mutantSpans);
        }

        public bool IsMatch(FilePattern pattern, string filePath, MutantSpan span)
        {
            // Check if the file path is matched.
            if (!pattern.Glob.IsMatch(FilePathUtils.NormalizePathSeparators(filePath)))
            {
                return false;
            }

            return IsMatch(pattern, span);
        }

        public abstract MutantSpan ToMutantSpan(TSpan span);

        public abstract TSpan FromMutantSpan(MutantSpan span);

        public abstract IEnumerable<TSpan> Reduce(IEnumerable<TSpan> spans);

        public abstract IEnumerable<TSpan> RemoveOverlap(IEnumerable<TSpan> left, IEnumerable<TSpan> right);

        public abstract bool IsMatch(FilePattern pattern, MutantSpan span);
    }
}
