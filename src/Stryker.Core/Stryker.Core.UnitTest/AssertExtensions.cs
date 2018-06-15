using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using System;

namespace Stryker.Core.UnitTest
{
    /// <summary>
    /// Making asserting syntax trees more easy
    /// </summary>
    public static class AssertionExtensions
    {
        /// <summary>
        /// Compares two code strings and asserts equality 
        /// </summary>
        /// <param name="actual">Resulted code</param>
        /// <param name="expected">Expected code</param>
        public static void ShouldBeSemantically(this string actual, string expected)
        {
            ShouldBeSemantically(CSharpSyntaxTree.ParseText(actual).GetRoot(), CSharpSyntaxTree.ParseText(expected).GetRoot());
        }

        /// <summary>
        /// Compares two syntax trees and asserts equality 
        /// </summary>
        /// <param name="actual">Resulted code</param>
        /// <param name="expected">Expected code</param>
        public static void ShouldBeSemantically(this SyntaxNode actual, SyntaxNode expected)
        {
            SyntaxFactory.AreEquivalent(actual, expected)
                .ShouldBeTrue($"AST's are not equavalent. Actual: {Environment.NewLine}{actual}, expected: {Environment.NewLine}{expected}");
        }

        public static void ShouldBeWithNewlineReplace(this string actual, string expected)
        {
            var iets = actual.Contains("\r\n");
            actual = actual.Replace("\r\n", Environment.NewLine);
            actual.ShouldBe(expected);
        }
    }
}
