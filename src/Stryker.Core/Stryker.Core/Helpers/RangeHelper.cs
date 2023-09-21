using System.Collections.Generic;
using System.Linq;
using FSharp.Compiler.Text;

namespace Stryker.Core.Helpers
{
    internal static class RangeHelper
    {
        /// <summary>
        /// Reduces a set of ranges to the smallest set of ranges possible.
        /// Two <see cref="Range" /> can be combined if they intersect.
        /// </summary>
        /// <param name="ranges">The set of <see cref="TextSpan" />s to reduce.</param>
        /// <returns>The reduced set.</returns>
        public static IReadOnlyCollection<Range> Reduce(string filePath, IEnumerable<Range> ranges)
        {
            var rangeList = new List<Range>(ranges);
            var shouldContinue = true;

            while (shouldContinue)
            {
                shouldContinue = false;

                foreach (var current in rangeList)
                {
                    // Check if any of the other spans intersects with the current one
                    var other = rangeList.FirstOrDefault(s => !RangeModule.equals(s, current) && s.IntersectsWith(current));
                    if (!RangeModule.equals(other, Range.Zero))
                    {
                        // Remove the original spans
                        rangeList.Remove(current);
                        rangeList.Remove(other);

                        // Add the newly combined span.
                        rangeList.Add(FromBounds(filePath, Min(current.Start, other.Start), Max(current.End, other.End)));

                        // We changed the list, so we have to restart the foreach.
                        shouldContinue = true;
                        break;
                    }
                }
            }

            return rangeList.Distinct().Where(x => !x.IsEmpty()).ToList();
        }

        internal static Position Max(Position pos1, Position pos2)
        {
            return PositionModule.posGeq(pos1, pos2) ? pos1 : pos2;
        }

        internal static Position Min(Position pos1, Position pos2)
        {
            return PositionModule.posLt(pos1, pos2) ? pos1 : pos2;
        }

        internal static bool IsEmpty(this Range range)
        {
            return RangeModule.equals(range, Range.Zero);
        }

        internal static bool IntersectsWith(this Range range1, Range range2)
        {
            return PositionModule.posGeq(range1.End, range2.Start)
                && PositionModule.posGeq(range2.End, range1.Start);
        }

        internal static Position GetPosition(string text, int index)
        {
            var line = 0;
            var col = 0;

            for (var i = 0; i < System.Math.Min(index, text.Length); i++)
            {
                if (text[i] == '\n')
                {
                    line++;
                    col = 0;
                }
                else
                {
                    col++;
                }
            }

            return PositionModule.mkPos(line, col);
        }

        internal static int GetIndex(string text, Position pos)
        {
            var line = 0;
            var col = 0;

            for (var i = 0; i < text.Length; i++)
            {
                if (line == pos.Line && col == pos.Column)
                {
                    return i;
                }

                if (text[i] == '\n')
                {
                    line++;
                    col = 0;
                }
                else
                {
                    col++;
                }
            }

            return -1;
        }

        public static Range FromBounds(string filePath, Position startPos, Position endPos)
        {
            return RangeModule.mkRange(filePath, startPos, endPos);
        }

        public static Range FromBounds(string filePath, string text, int startIndex, int endIndex)
        {
            var startPos = GetPosition(text, startIndex);
            var endPos = GetPosition(text, endIndex);
            return RangeModule.mkRange(filePath, startPos, endPos);
        }
    }
}
