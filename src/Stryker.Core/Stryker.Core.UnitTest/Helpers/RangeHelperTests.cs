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
        public void IsEmpty_EmptyRange()
        {
            var range = Range.Zero;

            range.IsEmpty().ShouldBeTrue();
        }

        [Fact]
        public void IsEmpty_NotEmptyRange()
        {
            var range = RangeModule.mkRange(
                "test.fs",
                PositionModule.pos0,
                PositionModule.mkPos(42, 42));

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
        public void IntersectsWith_Intersecting_Left()
        {
            var range1 = RangeModule.mkRange(
                "test.fs",
                PositionModule.pos0,
                PositionModule.mkPos(22, 22));

            var range2 = RangeModule.mkRange(
                "test.fs",
                PositionModule.mkPos(11, 11),
                PositionModule.mkPos(33, 33));

            range1.IntersectsWith(range2).ShouldBeTrue();
        }

        [Fact]
        public void IntersectsWith_Intersecting_Right()
        {
            var range1 = RangeModule.mkRange(
                "test.fs",
                PositionModule.mkPos(11, 11),
                PositionModule.mkPos(33, 33));

            var range2 = RangeModule.mkRange(
                "test.fs",
                PositionModule.mkPos(0, 0),
                PositionModule.mkPos(22, 22));

            range1.IntersectsWith(range2).ShouldBeTrue();
        }

        [Fact]
        public void IntersectsWith_Intersecting_Between()
        {
            var range1 = RangeModule.mkRange(
                "test.fs",
                PositionModule.mkPos(0, 0),
                PositionModule.mkPos(33, 33));

            var range2 = RangeModule.mkRange(
                "test.fs",
                PositionModule.mkPos(11, 11),
                PositionModule.mkPos(22, 22));

            range1.IntersectsWith(range2).ShouldBeTrue();
        }

        [Fact]
        public void IntersectsWith_NotIntersecting()
        {
            var range1 = RangeModule.mkRange(
                "test.fs",
                PositionModule.mkPos(0, 0),
                PositionModule.mkPos(11, 11));

            var range2 = RangeModule.mkRange(
                "test.fs",
                PositionModule.mkPos(22, 22),
                PositionModule.mkPos(33, 33));

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

            var range = RangeModule.mkRange(
                filePath,
                PositionModule.mkPos(0, 0),
                PositionModule.mkPos(42, 42));

            var ranges = new[] { range }; 

            var result = RangeHelper.Reduce(filePath, ranges);

            result.ShouldBe(ranges);
        }

        [Fact]
        public void Reduce_Two_Intersecting()
        {
            var filePath = "test.fs";

            var range1 = RangeModule.mkRange(
                filePath,
                PositionModule.mkPos(0, 0),
                PositionModule.mkPos(22, 22));

            var range2 = RangeModule.mkRange(
                filePath,
                PositionModule.mkPos(11, 11),
                PositionModule.mkPos(33, 33));

            var expectedRange = RangeModule.mkRange(
                filePath,
                PositionModule.mkPos(0, 0),
                PositionModule.mkPos(33, 33));

            var result = RangeHelper.Reduce(filePath, new[] { range1, range2 });

            result.ShouldBe(new[] { expectedRange });
        }

        [Fact]
        public void Reduce_Two_NonIntersecting()
        {
            var filePath = "test.fs";

            var range1 = RangeModule.mkRange(
                filePath,
                PositionModule.mkPos(0, 0),
                PositionModule.mkPos(11, 11));

            var range2 = RangeModule.mkRange(
                filePath,
                PositionModule.mkPos(22, 22),
                PositionModule.mkPos(33, 33));

            var ranges = new[] { range1, range2 };

            var actual = RangeHelper.Reduce(filePath, ranges);

            actual.ShouldBe(ranges);
        }

        [Fact]
        public void Reduce_Three_PartiallyIntersecting()
        {
            var filePath = "test.fs";

            var range1 = RangeModule.mkRange(
                filePath,
                PositionModule.mkPos(0, 0),
                PositionModule.mkPos(22, 22));

            var range2 = RangeModule.mkRange(
                filePath,
                PositionModule.mkPos(11, 11),
                PositionModule.mkPos(33, 33));

            var range3 = RangeModule.mkRange(
                filePath,
                PositionModule.mkPos(44, 44),
                PositionModule.mkPos(55, 55));

            var expectedIntersection = RangeModule.mkRange(
                filePath,
                PositionModule.mkPos(0, 0),
                PositionModule.mkPos(33, 33));

            var expectedRanges = new[] { expectedIntersection, range3 };

            var result = RangeHelper.Reduce(filePath, new[] { range1, range2, range3 }); ;

            result.ShouldBe(expectedRanges, ignoreOrder: true);
        }
    }
}
