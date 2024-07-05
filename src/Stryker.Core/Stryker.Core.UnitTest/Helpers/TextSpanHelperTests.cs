using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Stryker.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Helpers
{
    [TestClass]
    public class TextSpanHelperTests : TestBase
    {
        [TestMethod]
        [DataRow(new int[0], new int[0])]
        [DataRow(new[] { 5, 5 }, new int[0])]
        [DataRow(new[] { 5, 10 }, new[] { 5, 10 })]
        [DataRow(new[] { 5, 10, 5, 10 }, new[] { 5, 10 })]
        [DataRow(new[] { 5, 10, 10, 15 }, new[] { 5, 15 })]
        [DataRow(new[] { 5, 10, 10, 15, 15, 20 }, new[] { 5, 20 })]
        [DataRow(new[] { 5, 15, 10, 20 }, new[] { 5, 20 })]
        [DataRow(new[] { 5, 10, 15, 25 }, new[] { 5, 10, 15, 25 })]
        [DataRow(new[] { 5, 10, 10, 15, 20, 25, 25, 30 }, new[] { 5, 15, 20, 30 })]
        public void Reduce_should_reduce_correctly(int[] inputSpans, int[] outputSpans)
        {
            // Arrange
            var textSpans = ConvertToSpans(inputSpans);

            // Act
            var result = textSpans.Reduce();

            // Assert
            result.SequenceEqual(ConvertToSpans(outputSpans)).ShouldBeTrue();
        }

        [TestMethod]
        [DataRow(new int[0], new int[0], new int[0])]
        [DataRow(new[] { 5, 15 }, new[] { 5, 10 }, new[] { 10, 15 })]
        [DataRow(new[] { 5, 25 }, new[] { 5, 10, 10, 15 }, new[] { 15, 25 })]
        [DataRow(new[] { 5, 25 }, new[] { 5, 10, 15, 25 }, new[] { 10, 15 })]
        [DataRow(new[] { 5, 25 }, new[] { 5, 10, 15, 20 }, new[] { 10, 15, 20, 25 })]
        [DataRow(new[] { 5, 10 }, new[] { 5, 10 }, new int[0])]
        [DataRow(new[] { 5, 10 }, new[] { 0, 100 }, new int[0])]
        [DataRow(new[] { 5, 25, 50, 100 }, new[] { 20, 75 }, new[] { 5, 20, 75, 100 })]
        public void Remove_overlap_should_remove_overlap_correctly(int[] leftSpans, int[] rightSpans, int[] outputSpans)
        {
            // Act
            var result = ConvertToSpans(leftSpans).RemoveOverlap(ConvertToSpans(rightSpans));

            // Assert
            result.SequenceEqual(ConvertToSpans(outputSpans)).ShouldBeTrue();
        }

        private static IEnumerable<TextSpan> ConvertToSpans(int[] positions)
        {
            return Enumerable.Range(0, positions.Length)
                .GroupBy(i => Math.Floor(i / 2d))
                .Select(x => TextSpan.FromBounds(positions[x.First()], positions[x.Skip(1).First()]));
        }
    }
}
