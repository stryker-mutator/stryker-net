using System.Collections.Generic;
using System.IO;
using System.Linq;
using FSharp.Compiler.Text;

namespace Stryker.Core.Helpers
{
    public static class RangeHelper
    {
        /// <summary>
        /// Reduces a set of ranges to the smallest set of ranges possible.
        /// Two <see cref="Range" /> can be combined if they intersect.
        /// </summary>
        /// <param name="ranges">The set of <see cref="Range" />s to reduce.</param>
        /// <returns>The reduced set.</returns>
        public static IReadOnlyCollection<Range> Reduce(this IEnumerable<Range> ranges, string filePath)
        {
            var rangeList = new List<Range>(ranges);
            var shouldContinue = true;

            while (shouldContinue)
            {
                shouldContinue = false;

                foreach (var current in rangeList)
                {
                    // Check if any of the other ranges intersects with the current one
                    var other = rangeList.Find(s => !RangeModule.equals(s, current) && s.IntersectsWith(current));
                    if (!RangeModule.equals(other, Range.Zero))
                    {
                        // Remove the original ranges
                        rangeList.Remove(current);
                        rangeList.Remove(other);

                        // Add the newly combined range.
                        rangeList.Add(FromBounds(filePath, Min(current.Start, other.Start), Max(current.End, other.End)));

                        // We changed the list, so we have to restart the foreach.
                        shouldContinue = true;
                        break;
                    }
                }
            }

            return rangeList.Distinct().Where(x => !x.IsEmpty()).ToList();
        }

        /// <summary>
        /// Removes all overlaps of two sets of <see cref="Range"/> and returns the resulting set.
        /// </summary>
        /// <param name="left">The first set.</param>
        /// <param name="right">The second set.</param>
        /// <returns>All ranges and part of ranges of <paramref name="left"/> that do not overlap with any ranges in <paramref name="right"/>.</returns>
        public static IReadOnlyCollection<Range> RemoveOverlap(this IEnumerable<Range> left, IEnumerable<Range> right, string filePath)
        {
            var rangeList = new List<Range>(left);
            var shouldContinue = true;

            while (shouldContinue)
            {
                shouldContinue = false;

                foreach (var current in rangeList)
                {
                    // Check if any range overlaps the current range.
                    var other = right.FirstOrDefault(o => o.OverlapsWith(current));

                    if (!RangeModule.equals(other, Range.Zero))
                    {
                        // Remove the current range add the new range(s).
                        rangeList.Remove(current);
                        rangeList.AddRange(RemoveOverlap(current, other));

                        // We changed the list, so we have to restart the foreach.
                        shouldContinue = true;
                        break;
                    }
                }
            }

            return rangeList;

            IReadOnlyCollection<Range> RemoveOverlap(Range current, Range other)
            {
                // The the current range is completely contained inside the other, nothing will be left.
                if (RangeModule.rangeContainsRange(other, current))
                    return System.Array.Empty<Range>();

                // Check if there is any overlap.
                var overlap = current.Overlap(other, filePath);

                if (!overlap.HasValue)
                {
                    return new[] { current };
                }

                return new[] { FromBounds(filePath, current.Start, overlap.Value.Start), FromBounds(filePath, overlap.Value.End, current.End) }.Where(s => !s.IsEmpty()).ToList();
            }
        }

        public static bool OverlapsWith(this Range range1, Range range2)
        {
            var overlapStart = Max(range1.Start, range2.Start);
            var overlapEnd = Min(range1.End, range2.End);

            return PositionModule.posLt(overlapStart, overlapEnd);
        }

        public static Range? Overlap(this Range range1, Range range2, string filePath)
        {
            var overlapStart = Max(range1.Start, range2.Start);
            var overlapEnd = Min(range1.End, range2.End);

            return PositionModule.posLt(overlapStart, overlapEnd)
                ? FromBounds(filePath, overlapStart, overlapEnd)
                : null;
        }

        public static Position Max(Position pos1, Position pos2)
            => PositionModule.posGeq(pos1, pos2) ? pos1 : pos2;

        public static Position Min(Position pos1, Position pos2)
            => PositionModule.posLt(pos1, pos2) ? pos1 : pos2;

        public static bool IsEmpty(this Range range)
            => PositionModule.posEq(range.Start, range.End);

        public static bool IntersectsWith(this Range range1, Range range2)
            => PositionModule.posGeq(range1.End, range2.Start)
            && PositionModule.posGeq(range2.End, range1.Start);

        public static Position GetPosition(string text, int index)
        {
            var line = 0;

            using var reader = new StringReader(text);
            var currentIndex = 0;
            do
            {
                string? latestLineContent = reader.ReadLine();
                if (latestLineContent == null) break;

                var lengthOfThisLine = latestLineContent.Length;
                var endOfLineIndex = currentIndex + lengthOfThisLine;
                var indexIsOnThisLine = index <= endOfLineIndex && index >= currentIndex;
                if (indexIsOnThisLine)
                {
                    var indexOnThisLine = index - currentIndex;
                    return PositionModule.mkPos(line, indexOnThisLine);
                }

                var thisWasTheLastLine = reader.Peek() == -1;
                if (thisWasTheLastLine)
                {
                    return PositionModule.mkPos(line, lengthOfThisLine);
                }

                line++;
                currentIndex += lengthOfThisLine;
            } while (currentIndex < text.Length);

            return PositionModule.mkPos(0, 0);
        }

        public static int GetIndex(string text, Position pos)
        {
            using var reader = new StringReader(text);
            string? latestLineContent = "";
            var currentIndex = 0;
            for (var line = 0; line <= pos.Line; ++line)
            {
                if (latestLineContent == null)
                {
                    break;
                }
                currentIndex += latestLineContent.Length;
                latestLineContent = reader.ReadLine();
            }

            if (latestLineContent != null && pos.Column < latestLineContent.Length)
            {
                return currentIndex + pos.Column;
            }

            return -1;
        }

        public static Range FromBounds(string filePath, Position startPos, Position endPos)
            => RangeModule.mkRange(filePath, startPos, endPos);

        public static Range FromBounds(string filePath, string text, int startIndex, int endIndex)
        {
            var startPos = GetPosition(text, startIndex);
            var endPos = GetPosition(text, endIndex);
            return RangeModule.mkRange(filePath, startPos, endPos);
        }
    }
}
