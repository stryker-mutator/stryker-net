using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using System;
using System.Linq;

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
            // for some reason, nodes can be different while being textually the same
            if (actual.ToFullString() == expected.ToFullString())
            {
                return;
            }
            var issame = SyntaxFactory.AreEquivalent(actual, expected);

            if (!issame)
            {
                // find the different
                var actuaLines = actual.ToString().Split(Environment.NewLine);
                var expectedLines = expected.ToString().Split(Environment.NewLine);
                for(var i = 0; i < actuaLines.Length; i++)
                {
                    if (actuaLines[i] != expectedLines[i])
                    {
                        issame.ShouldBeTrue($"AST's are not equivalent. Line[{i+1}]{Environment.NewLine}actual:{actuaLines[i]}{Environment.NewLine}expect:{expectedLines[i]}{Environment.NewLine}Actual(full):{Environment.NewLine}{actual}{Environment.NewLine}, expected:{Environment.NewLine}{expected}");
                    }
                }
            }
        }

        public static void ShouldBeWithNewlineReplace(this string actual, string expected)
        {
            string replaced = expected.Replace("\r\n", Environment.NewLine, StringComparison.InvariantCultureIgnoreCase);
            actual.ShouldBe(replaced);
        }
        
        public static void ShouldNotContainErrors(this SyntaxNode actual)
        {
            var errors = actual.SyntaxTree.GetDiagnostics().Count(x => x.Severity == DiagnosticSeverity.Error);
            if (errors > 0)
            {
                errors.ShouldBe(0, $"errors: {string.Join(Environment.NewLine, actual.SyntaxTree.GetDiagnostics())}");
            }
        }
    }
}
