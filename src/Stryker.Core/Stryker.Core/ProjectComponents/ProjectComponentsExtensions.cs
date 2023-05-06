using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.ProjectComponents;

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

    /// <summary>
    /// Reduces a set of text spans to the smallest set of text spans possible.
    /// Two <see cref="TextSpan" /> can be combined if they intersect.
    /// </summary>
    /// <param name="textSpans">The set of <see cref="TextSpan" />s to reduce.</param>
    /// <returns>The reduced set.</returns>
    public static IReadOnlyCollection<TextSpan> Reduce(this IEnumerable<TextSpan> textSpans)
    {
        var spans = new List<TextSpan>(textSpans);
        var shouldContinue = true;

        while (shouldContinue)
        {
            shouldContinue = false;

            foreach (var current in spans)
            {
                // Check if any of the other spans intersects with the current one
                var other = spans.FirstOrDefault(s => s != current && s.IntersectsWith(current));
                if (other != default)
                {
                    // Remove the original spans
                    spans.Remove(current);
                    spans.Remove(other);

                    // Add the newly combined span.
                    spans.Add(TextSpan.FromBounds(Math.Min(current.Start, other.Start), Math.Max(current.End, other.End)));

                    // We changed the list, so we have to restart the foreach.
                    shouldContinue = true;
                    break;
                }
            }
        }

        return spans.Distinct().Where(x => !x.IsEmpty).ToList();
    }

    /// <summary>
    /// Removes all overlaps of two sets of <see cref="TextSpan"/> and returns the resulting set.
    /// </summary>
    /// <param name="left">The first set.</param>
    /// <param name="right">The second set.</param>
    /// <returns>All spans and part of spans of <paramref name="left"/> that do not overlap with any span in <paramref name="right"/>.</returns>
    public static IReadOnlyCollection<TextSpan> RemoveOverlap(this IEnumerable<TextSpan> left, IEnumerable<TextSpan> right)
    {
        var spanList = new List<TextSpan>(left);
        var shouldContinue = true;

        while (shouldContinue)
        {
            shouldContinue = false;

            foreach (var current in spanList)
            {
                // Check if any span overlaps the current span.
                var other = right.FirstOrDefault(o => o.OverlapsWith(current));

                if (other != default)
                {
                    // Remove the current span add the new span(s).
                    spanList.Remove(current);
                    spanList.AddRange(RemoveOverlap(current, other));

                    // We changed the list, so we have to restart the foreach.
                    shouldContinue = true;
                    break;
                }
            }
        }

        return spanList;

        IReadOnlyCollection<TextSpan> RemoveOverlap(TextSpan current, TextSpan other)
        {
            // The the current span is completely contained inside the other, nothing will be left.
            if (other.Contains(current))
                return Array.Empty<TextSpan>();

            // Check if there is any overlap.
            var overlap = current.Overlap(other);

            if (!overlap.HasValue)
            {
                return new[] { current };
            }

            return new[] { TextSpan.FromBounds(current.Start, overlap.Value.Start), TextSpan.FromBounds(overlap.Value.End, current.End) }.Where(s => !s.IsEmpty).ToList();
        }
    }
}
