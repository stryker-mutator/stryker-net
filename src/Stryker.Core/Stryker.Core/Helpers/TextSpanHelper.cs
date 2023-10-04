using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Text;

namespace Stryker.Core.Helpers
{
    public static class TextSpanHelper
    {
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
                    var other = spans.Find(s => s != current && s.IntersectsWith(current));
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
}
