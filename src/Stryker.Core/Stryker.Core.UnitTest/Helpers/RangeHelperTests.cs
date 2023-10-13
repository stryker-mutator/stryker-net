using System;
using System.Linq;
using FSharp.Compiler.Text;
using Shouldly;
using Stryker.Core.Helpers;
using Xunit;
using Range = FSharp.Compiler.Text.Range;


namespace Stryker.Core.UnitTest.Helpers
{
    public class RangeHelperTests : TestBase
    {
        [Fact]
        public void Reduce_Empty()
        {
            var result = Enumerable.Empty<Range>().Reduce("test.fs");

            result.ShouldBeEmpty();
        }

        [Fact]
        public void Reduce_Hollow()
        {
            var range = GetRange((42, 42), (42, 42));

            var result = new[] { range }.Reduce("test.fs");

            result.ShouldBeEmpty();
        }

        [Fact]
        public void Reduce_One()
        {
            var range = GetRange((0, 0), (42, 42));

            var result = new[] { range }.Reduce("test.fs");

            result.ShouldBe(new[] { range });
        }

        [Fact]
        public void Reduce_TwoEqual()
        {
            var range1 = GetRange((0, 0), (42, 42));
            var range2 = GetRange((0, 0), (42, 42));

            var result = new[] { range1, range2 }.Reduce("test.fs");

            result.ShouldBe(new[] { range1 });
        }

        [Fact]
        public void Reduce_TwoSequential()
        {
            var range1 = GetRange((0, 0), (11, 11));
            var range2 = GetRange((0, 0), (22, 22));
            var merged = GetRange((0, 0), (22, 22));

            var result = new[] { range1, range2 }.Reduce("test.fs");

            result.ShouldBe(new[] { merged });
        }

        [Fact]
        public void Reduce_TwoIntersecting()
        {
            var range1 = GetRange((0, 0), (22, 22));
            var range2 = GetRange((11, 11), (33, 33));
            var intersection = GetRange((0, 0), (33, 33));

            var result = new[] { range1, range2 }.Reduce("test.fs");

            result.ShouldBe(new[] { intersection });
        }

        [Fact]
        public void Reduce_TwoNotIntersecting()
        {
            var range1 = GetRange((0, 0), (11, 11));
            var range2 = GetRange((22, 22), (33, 33));

            var result = new[] { range1, range2 }.Reduce("test.fs");

            result.ShouldBe(new[] { range1, range2 });
        }

        [Fact]
        public void Reduce_ThreeSequential()
        {
            var range1 = GetRange((0, 0), (11, 11));
            var range2 = GetRange((11, 11), (22, 22));
            var range3 = GetRange((22, 22), (33, 33));
            var merged = GetRange((0, 0), (33, 33));

            var result = new[] { range1, range2, range3 }.Reduce("test.fs");

            result.ShouldBe(new[] { merged });
        }

        [Fact]
        public void Reduce_ThreePartiallyIntersecting()
        {
            var range1 = GetRange((0, 0), (22, 22));
            var range2 = GetRange((11, 11), (33, 33));
            var range3 = GetRange((44, 44), (55, 55));
            var intersection = GetRange((0, 0), (33, 33));

            var result = new[] { range1, range2, range3 }.Reduce("test.fs");

            result.ShouldBe(new[] { intersection, range3 }, ignoreOrder: true);
        }

        [Fact]
        public void RemoveOverlap_Empty()
        {
            var result = Enumerable.Empty<Range>().RemoveOverlap(Enumerable.Empty<Range>(), "test.fs");

            result.ShouldBeEmpty();
        }

        [Fact]
        public void RemoveOverlap_OverlappingPartiallyLeft()
        {
            var range1 = GetRange((0, 0), (22, 22));
            var range2 = GetRange((11, 11), (22, 22));
            var rangeWithoutOverlap = GetRange((0, 0), (11, 11));

            var result = new[] { range1 }.RemoveOverlap(new[] { range2 }, "test.fs");

            result.ShouldBe(new[] { rangeWithoutOverlap });
        }

        [Fact]
        public void RemoveOverlap_OverlappingPartiallyRight()
        {
            var range1 = GetRange((0, 0), (22, 22));
            var range2 = GetRange((0, 0), (11, 11));
            var rangeWithoutOverlap = GetRange((11, 11), (22, 22));

            var result = new[] { range1 }.RemoveOverlap(new[] { range2 }, "test.fs");

            result.ShouldBe(new[] { rangeWithoutOverlap });
        }

        [Fact]
        public void RemoveOverlap_OverlappingBySequentialRanges()
        {
            var range1 = GetRange((0, 0), (33, 33));
            var range2 = GetRange((0, 0), (11, 11));
            var range3 = GetRange((11, 11), (22, 22));
            var rangeWithoutOverlap = GetRange((22, 22), (33, 33));

            var result = new[] { range1 }.RemoveOverlap(new[] { range2, range3 }, "test.fs");

            result.ShouldBe(new[] { rangeWithoutOverlap });
        }

        [Fact]
        public void RemoveOverlap_OverlappingByRangesFromSides()
        {
            var range1 = GetRange((0, 0), (33, 33));
            var range2 = GetRange((0, 0), (11, 11));
            var range3 = GetRange((22, 22), (33, 33));
            var rangeWithoutOverlap = GetRange((11, 11), (22, 22));

            var result = new[] { range1 }.RemoveOverlap(new[] { range2, range3 }, "test.fs");

            result.ShouldBe(new[] { rangeWithoutOverlap });
        }

        [Fact]
        public void RemoveOverlap_OverlappingCompletely()
        {
            var range1 = GetRange((0, 0), (42, 42));
            var range2 = GetRange((0, 0), (42, 42));

            var result = new[] { range1 }.RemoveOverlap(new[] { range2 }, "test.fs");

            result.ShouldBeEmpty();
        }

        [Fact]
        public void RemoveOverlap_OverlappingByBigger()
        {
            var range1 = GetRange((0, 0), (11, 11));
            var range2 = GetRange((0, 0), (44, 44));

            var result = new[] { range1 }.RemoveOverlap(new[] { range2 }, "test.fs");

            result.ShouldBeEmpty();
        }

        [Fact]
        public void RemoveOverlap_CutInTheMiddle()
        {
            var range1 = GetRange((0, 0), (33, 33));
            var range2 = GetRange((11, 11), (22, 22));
            var leftPart = GetRange((0, 0), (11, 11));
            var rightPart = GetRange((22, 22), (33, 33));

            var result = new[] { range1 }.RemoveOverlap(new[] { range2 }, "test.fs");

            result.ShouldBe(new[] { leftPart, rightPart });
        }

        [Fact]
        public void RemoveOverlap_NotOverlapping()
        {
            var range1 = GetRange((0, 0), (11, 11));
            var range2 = GetRange((22, 22), (33, 33));

            var result = new[] { range1 }.RemoveOverlap(new[] { range2 }, "test.fs");

            result.ShouldBe(new[] { range1 });
        }

        [Fact]
        public void OverlapsWith_OverlappingLeft()
        {
            var range1 = GetRange((0, 0), (22, 22));
            var range2 = GetRange((11, 11), (33, 33));

            range1.OverlapsWith(range2).ShouldBeTrue();
        }

        [Fact]
        public void OverlapsWith_OverlappingRight()
        {
            var range1 = GetRange((11, 11), (33, 33));
            var range2 = GetRange((0, 0), (22, 22));

            range1.OverlapsWith(range2).ShouldBeTrue();
        }

        [Fact]
        public void OverlapsWith_OverlappingBetween()
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
        public void OverlapsWith_EmptyLeft()
        {
            var range1 = GetRange((0, 0), (42, 42));
            var range2 = Range.Zero;

            range1.OverlapsWith(range2).ShouldBeFalse();
        }

        [Fact]
        public void OverlapsWith_EmptyRight()
        {
            var range1 = Range.Zero;
            var range2 = GetRange((0, 0), (42, 42));

            range1.OverlapsWith(range2).ShouldBeFalse();
        }

        [Fact]
        public void OverlapsWith_EmptyBoth()
        {
            var range1 = Range.Zero;
            var range2 = Range.Zero;

            range1.OverlapsWith(range2).ShouldBeFalse();
        }

        [Fact]
        public void Overlap_Overlapping()
        {
            var range1 = GetRange((0, 0), (22, 22));
            var range2 = GetRange((11, 11), (33, 33));
            var overlap = GetRange((11, 11), (22, 22));

            range1.Overlap(range2, "test.fs").ShouldBe(overlap);
        }

        [Fact]
        public void Overlap_Sequential()
        {
            var range1 = GetRange((0, 0), (11, 11));
            var range2 = GetRange((11, 11), (22, 22));

            range1.Overlap(range2, "test.fs").ShouldBeNull();
        }

        [Fact]
        public void Overlap_NotOverlapping()
        {
            var range1 = GetRange((0, 0), (11, 11));
            var range2 = GetRange((22, 22), (33, 33));

            range1.Overlap(range2, "test.fs").ShouldBeNull();
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
        public void IntersectsWith_Intersecting_Left()
        {
            var range1 = GetRange((0, 0), (22, 22));
            var range2 = GetRange((11, 11), (33, 33));

            range1.IntersectsWith(range2).ShouldBeTrue();
        }

        [Fact]
        public void IntersectsWith_IntersectingRight()
        {
            var range1 = GetRange((11, 11), (33, 33));
            var range2 = GetRange((0, 0), (22, 22));

            range1.IntersectsWith(range2).ShouldBeTrue();
        }

        [Fact]
        public void IntersectsWith_IntersectingBetween()
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
        public void GetPosition_OneLine()
        {
            var text = "Line1";
            var index = 1;
            var position = PositionModule.mkPos(0, 1);

            var result = RangeHelper.GetPosition(text, index);

            result.ShouldBe(position);
        }

        [Fact]
        public void GetPosition_NoContent()
        {
            var text = "";
            var index = 0;
            var position = PositionModule.mkPos(0, 0);

            var result = RangeHelper.GetPosition(text, index);

            result.ShouldBe(position);
        }

        [Fact]
        public void GetPosition_NoContent_OutOfBounds()
        {
            var text = "";
            var index = 42;
            var position = PositionModule.mkPos(0, 0);

            var result = RangeHelper.GetPosition(text, index);

            result.ShouldBe(position);
        }

        [Theory]
        [InlineData(1, 0, 1)]
        [InlineData(4, 0, 4)]
        [InlineData(7, 1, 2)]
        [InlineData(9, 1, 4)]
        public void GetPosition_MultipleLines(int index, int expectedRow, int expectedCol)
        {
            var text = $"Line1{Environment.NewLine}Line2";
            var position = PositionModule.mkPos(expectedRow, expectedCol);

            var result = RangeHelper.GetPosition(text, index);

            result.ShouldBe(position);
        }

        [Fact]
        public void GetPosition_OutOfBounds()
        {
            var text = "Line1";
            var index = 42;
            var position = PositionModule.mkPos(0, 5);

            var result = RangeHelper.GetPosition(text, index);

            result.ShouldBe(position);
        }

        [Fact]
        public void GetIndex_OneLine() => GetIndex("Line1", expectedIndex: 1, row: 0, col: 1);

        [Theory]
        [InlineData(1, 0, 1)]
        [InlineData(4, 0, 4)]
        [InlineData(7, 1, 2)]
        [InlineData(9, 1, 4)]

        public void GetIndex_MultipleLines(int expectedIndex, int row, int col) => GetIndex($"Line1{Environment.NewLine}Line2", expectedIndex, row, col);

        [Theory]
        [InlineData(-1, 0, 42)]
        [InlineData(-1, 1, 42)]
        [InlineData(-1, 2, 0)]
        [InlineData(-1, 2, 42)]
        public void GetIndex_MultipleLines_OutOfBounds(int expectedIndex, int row, int col) => GetIndex($"Line1{Environment.NewLine}Line2", expectedIndex, row, col);

        [Fact]
        public void GetIndex_OutOfBounds() => GetIndex("Line1", expectedIndex: -1, row: 42, col: 42);

        private static void GetIndex(string text, int expectedIndex, int row, int col)
        {
            var position = PositionModule.mkPos(row, col);

            var result = RangeHelper.GetIndex(text, position);

            result.ShouldBe(expectedIndex);
        }

        private static Range GetRange((int Line, int Column) start, (int Line, int Column) end) =>
            RangeModule.mkRange(
                "test.fs",
                PositionModule.mkPos(start.Line, start.Column),
                PositionModule.mkPos(end.Line, end.Column));
    }
}
