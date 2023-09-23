using System.Linq;
using FSharp.Compiler.Text;
using Shouldly;
using Stryker.Core.Helpers;
using Xunit;

namespace Stryker.Core.UnitTest.Helpers
{
    public class RangeHelperTests : TestBase
    {
        [Fact]
        public void IsEmpty_ZeroRange()
        {
            var range = Range.Zero;

            range.IsEmpty().ShouldBeTrue();
        }

        [Fact]
        public void IsEmpty_HollowRange()
        {
            var range = GetRange((42, 42), (42, 42));

            range.IsEmpty().ShouldBeTrue();
        }

        [Fact]
        public void IsEmpty_NotEmptyRange()
        {
            var range = GetRange((0, 0), (42, 42));

            range.IsEmpty().ShouldBeFalse();
        }

        [Fact]
        public void Max_Greater()
        {
            var position1 = PositionModule.mkPos(42, 42);
            var position2 = PositionModule.pos0;

            var actual = RangeHelper.Max(position1, position2);

            actual.ShouldBe(position1);
        }

        [Fact]
        public void Max_Equal()
        {
            var position1 = PositionModule.pos0;
            var position2 = PositionModule.pos0;

            var actual = RangeHelper.Max(position1, position2);

            actual.ShouldBe(position1);
        }

        [Fact]
        public void Max_Less()
        {
            var position1 = PositionModule.pos0;
            var position2 = PositionModule.mkPos(42, 42);

            var actual = RangeHelper.Max(position1, position2);

            actual.ShouldBe(position2);
        }

        [Fact]
        public void Min_Greater()
        {
            var position1 = PositionModule.mkPos(42, 42);
            var position2 = PositionModule.pos0;

            var actual = RangeHelper.Min(position1, position2);

            actual.ShouldBe(position2);
        }

        [Fact]
        public void Min_Equal()
        {
            var position1 = PositionModule.pos0;
            var position2 = PositionModule.pos0;

            var actual = RangeHelper.Min(position1, position2);

            actual.ShouldBe(position2);
        }

        [Fact]
        public void Min_Less()
        {
            var position1 = PositionModule.pos0;
            var position2 = PositionModule.mkPos(42, 42);

            var actual = RangeHelper.Min(position1, position2);

            actual.ShouldBe(position1);
        }

        [Fact]
        public void OverlapsWith_Overlapping_Left()
        {
            var range1 = GetRange((0, 0), (22, 22));
            var range2 = GetRange((11, 11), (33, 33));

            range1.OverlapsWith(range2).ShouldBeTrue();
        }

        [Fact]
        public void OverlapsWith_Overlapping_Right()
        {
            var range1 = GetRange((11, 11), (33, 33));
            var range2 = GetRange((0, 0), (22, 22));

            range1.OverlapsWith(range2).ShouldBeTrue();
        }

        [Fact]
        public void OverlapsWith_Overlapping_Between()
        {
            var range1 = GetRange((0, 0), (33, 33));
            var range2 = GetRange((11, 11), (22, 22));

            range1.OverlapsWith(range2).ShouldBeTrue();
        }

        [Fact]
        public void OverlapsWith_NotOverlapping()
        {
            var range1 = GetRange((0, 0), (11, 11));
            var range2 = GetRange((22, 22), (33, 33));

            range1.OverlapsWith(range2).ShouldBeFalse();
        }

        [Fact]
        public void OverlapsWith_Empty_Left()
        {
            var range1 = GetRange((0, 0), (42, 42));
            var range2 = Range.Zero;

            range1.OverlapsWith(range2).ShouldBeFalse();
        }

        [Fact]
        public void OverlapsWith_Empty_Right()
        {
            var range1 = Range.Zero;
            var range2 = GetRange((0, 0), (42, 42));

            range1.OverlapsWith(range2).ShouldBeFalse();
        }

        [Fact]
        public void OverlapsWith_Empty_Both()
        {
            var range1 = Range.Zero;
            var range2 = Range.Zero;

            range1.OverlapsWith(range2).ShouldBeFalse();
        }

        [Fact]
        public void Overlap_Overlapping()
        {
            var filePath = "test.fs";

            var range1 = GetRange((0, 0), (22, 22));
            var range2 = GetRange((11, 11), (33, 33));
            var expectedOverlap = GetRange((11, 11), (22, 22));

            range1.Overlap(range2, filePath).ShouldBe(expectedOverlap);
        }

        [Fact]
        public void Overlap_NotOverlapping()
        {
            var filePath = "test.fs";

            var range1 = GetRange((0, 0), (11, 11));
            var range2 = GetRange((22, 22), (33, 33));

            range1.Overlap(range2, filePath).ShouldBeNull();
        }

        [Fact]
        public void IntersectsWith_Intersecting_Left()
        {
            var range1 = GetRange((0, 0), (22, 22));
            var range2 = GetRange((11, 11), (33, 33));

            range1.IntersectsWith(range2).ShouldBeTrue();
        }

        [Fact]
        public void IntersectsWith_Intersecting_Right()
        {
            var range1 = GetRange((11, 11), (33, 33));
            var range2 = GetRange((0, 0), (22, 22));

            range1.IntersectsWith(range2).ShouldBeTrue();
        }

        [Fact]
        public void IntersectsWith_Intersecting_Between()
        {
            var range1 = GetRange((0, 0), (33, 33));
            var range2 = GetRange((11, 11), (22, 22));

            range1.IntersectsWith(range2).ShouldBeTrue();
        }

        [Fact]
        public void IntersectsWith_NoIntersection()
        {
            var range1 = GetRange((0, 0), (11, 11));
            var range2 = GetRange((22, 22), (33, 33));

            range1.IntersectsWith(range2).ShouldBeFalse();
        }

        [Fact]
        public void Reduce_Zero()
        {
            var result = RangeHelper.Reduce("test.fs", Enumerable.Empty<Range>());

            result.ShouldBeEmpty();
        }

        [Fact]
        public void Reduce_One()
        {
            var filePath = "test.fs";

            var range = GetRange((0, 0), (42, 42));

            var ranges = new[] { range }; 

            var result = RangeHelper.Reduce(filePath, ranges);

            result.ShouldBe(ranges);
        }

        [Fact]
        public void Reduce_Two_Intersecting()
        {
            var filePath = "test.fs";

            var range1 = GetRange((0, 0), (22, 22));
            var range2 = GetRange((11, 11), (33, 33));
            var expectedRange = GetRange((0, 0), (33, 33));

            var result = RangeHelper.Reduce(filePath, new[] { range1, range2 });

            result.ShouldBe(new[] { expectedRange });
        }

        [Fact]
        public void Reduce_Two_NonIntersecting()
        {
            var filePath = "test.fs";

            var range1 = GetRange((0, 0), (11, 11));
            var range2 = GetRange((22, 22), (33, 33));
            var ranges = new[] { range1, range2 };

            var actual = RangeHelper.Reduce(filePath, ranges);

            actual.ShouldBe(ranges);
        }

        [Fact]
        public void Reduce_Three_PartiallyIntersecting()
        {
            var filePath = "test.fs";

            var range1 = GetRange((0, 0), (22, 22));
            var range2 = GetRange((11, 11), (33, 33));
            var range3 = GetRange((44, 44), (55, 55));
            var expectedIntersection = GetRange((0, 0), (33, 33));
            var expectedRanges = new[] { expectedIntersection, range3 };

            var result = RangeHelper.Reduce(filePath, new[] { range1, range2, range3 }); ;

            result.ShouldBe(expectedRanges, ignoreOrder: true);
        }

        [Fact]
        public void RemoveOverlap_Overlapping_Partially()
        {
            var filePath = "test.fs";

            var range1 = GetRange((0, 0), (22, 22));
            var range2 = GetRange((11, 11), (22, 22));
            var expectedRange = GetRange((0, 0), (11, 11));

            var result = RangeHelper.RemoveOverlap(new[] { range1 }, new [] { range2 }, filePath);

            result.ShouldBe(new[] { expectedRange });
        }

        [Fact]
        public void RemoveOverlap_Overlapping_Completely()
        {
            var filePath = "test.fs";

            var range1 = GetRange((0, 0), (42, 42));
            var range2 = GetRange((0, 0), (42, 42));

            var result = RangeHelper.RemoveOverlap(new[] { range1 }, new[] { range2 }, filePath);

            result.ShouldBeEmpty();
        }

        [Fact]
        public void RemoveOverlap_NotOverlapping()
        {
            var filePath = "test.fs";

            var range1 = GetRange((0, 0), (11, 11));
            var range2 = GetRange((22, 22), (33, 33));

            var result = RangeHelper.RemoveOverlap(new[] { range1 }, new[] { range2 }, filePath);

            result.ShouldBe(new[] { range1 });
        }

        private static Range GetRange((int Line, int Column) start, (int Line, int Column) end) =>
            RangeModule.mkRange(
                "test.fs",
                PositionModule.mkPos(start.Line, start.Column),
                PositionModule.mkPos(end.Line, end.Column));
    }
}
