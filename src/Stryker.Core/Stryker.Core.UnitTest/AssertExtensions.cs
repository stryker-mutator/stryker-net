using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using System;
using System.IO.Abstractions.TestingHelpers;
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
        public static void ShouldBeSemantically(this string actual, string expected) => ShouldBeSemantically(CSharpSyntaxTree.ParseText(actual), CSharpSyntaxTree.ParseText(expected));

        /// <summary>
        /// Compares two syntax trees and asserts equality
        /// </summary>
        /// <param name="actual">Resulted code</param>
        /// <param name="expected">Expected code</param>
        /// <remarks>Warning: this code tries to pinpoint the first different lines, but it will work on string comparison, so it may pinpoint spaces
        /// or new line variations, hiding the real differences.</remarks>
        public static void ShouldBeSemantically(this SyntaxTree actual, SyntaxTree expected)
        {
            var isSame = actual.IsEquivalentTo(expected);

            if (!isSame)
            {
                // find the different
                var actualLines = actual.ToString().Split(Environment.NewLine);
                var expectedLines = expected.ToString().Split(Environment.NewLine);
                for (var i = 0; i < actualLines.Length; i++)
                {
                    if (expectedLines.Length <= i)
                    {
                        isSame.ShouldBeTrue($"AST's are not equivalent. Line[{i + 1}]{Environment.NewLine}actual:{actualLines[i]}{Environment.NewLine}expect: nothing{Environment.NewLine}Actual(full):{Environment.NewLine}{actual}{Environment.NewLine}, expected:{Environment.NewLine}{expected}");
                    }
                    if (actualLines[i] != expectedLines[i])
                    {
                        isSame.ShouldBeTrue($"AST's are not equivalent. Line[{i + 1}]{Environment.NewLine}actual:{actualLines[i]}{Environment.NewLine}expect:{expectedLines[i]}{Environment.NewLine}Actual(full):{Environment.NewLine}{actual}{Environment.NewLine}, expected:{Environment.NewLine}{expected}");
                    }
                }
            }
        }

        public static void ShouldBeWithNewlineReplace(this string actual, string expected)
        {
            string replaced = expected.Replace("\r\n", Environment.NewLine, StringComparison.InvariantCultureIgnoreCase);
            actual.ShouldBe(replaced);
        }

        public static void ShouldNotContainErrors(this SyntaxTree actual)
        {
            var errors = actual.GetDiagnostics().Count(x => x.Severity == DiagnosticSeverity.Error);
            if (errors > 0)
            {
                errors.ShouldBe(0, $"Actual code has build errors!\n{actual.GetText()}\nerrors: {string.Join(Environment.NewLine, actual.GetDiagnostics())}");
            }
        }

        public static void ShouldContainFile(this MockFileSystem fileSystem, string expectedFilePath)
        {
            if (!fileSystem.FileExists(expectedFilePath))
            {
                throw new ShouldAssertException($@"The expected file was not written to disk, or not to the right location.

Expected: ""{expectedFilePath}"".

Files found:
{string.Join($", {Environment.NewLine}", fileSystem.AllFiles)}");
            }
        }
        public static void ShouldNotContainFile(this MockFileSystem fileSystem, string expectedFilePath)
        {
            if (fileSystem.FileExists(expectedFilePath))
            {
                throw new ShouldAssertException($@"This file should not exist there.
Expected: ""{expectedFilePath}"".

Files found:
{string.Join($", {Environment.NewLine}", fileSystem.AllFiles)}");
            }
        }
    }
}
