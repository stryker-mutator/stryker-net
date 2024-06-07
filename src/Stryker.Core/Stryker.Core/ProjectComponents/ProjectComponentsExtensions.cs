using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Helpers;

namespace Stryker.Core.ProjectComponents
{
    /// <summary>
    /// Contains extension methods for project components.
    /// </summary>
    public static class IProjectComponentsExtensions
    {
        /// <summary>
        /// Checks with the given <see cref="ProjectComponent" />s whether all parts of the component are excluded.
        /// </summary>
        /// <param name="projectComponent">The file to check.</param>
        /// <param name="filePatterns">The file patters to check with.</param>
        /// <returns>If any parts of the file are included <c>false</c>; otherwise <c>true</c>.</returns>
        public static bool IsComponentExcluded(this IReadOnlyProjectComponent projectComponent, IEnumerable<FilePattern> filePatterns)
        {
            var includePattern = filePatterns.Where(x => !x.IsExclude).ToList();
            var excludePattern = filePatterns.Where(x => x.IsExclude).ToList();

            // Get in- and excluded spans
            var includedSpans = includePattern.Where(MatchesFilePattern).SelectMany(x => x.TextSpans).Reduce();
            var excludedSpans = excludePattern.Where(MatchesFilePattern).SelectMany(x => x.TextSpans).Reduce();

            // If there are only included spans, the file is not excluded.
            if ((includedSpans.Any() && !excludedSpans.Any()) || projectComponent.Parent?.Parent == null)
            {
                return false;
            }

            return !includedSpans.RemoveOverlap(excludedSpans).Any();

            bool MatchesFilePattern(FilePattern pattern) =>
                pattern.Glob.IsMatch(projectComponent.FullPath) ||
                pattern.Glob.IsMatch(projectComponent.RelativePath);
        }
    }
}
