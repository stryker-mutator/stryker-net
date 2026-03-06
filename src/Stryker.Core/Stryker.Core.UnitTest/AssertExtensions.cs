using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

namespace Stryker.Core.UnitTest;

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
    public static void ShouldBeSemantically(this string actual, string expected) => CSharpSyntaxTree.ParseText(actual).ShouldBeSemantically(CSharpSyntaxTree.ParseText(expected));

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
            var diff = ScanDiff(actual.GetRoot(), expected.GetRoot());

            Console.WriteLine();

            throw new ShouldAssertException("The actual syntax tree is not equivalent to the expected syntax tree. Differences:"+string.Join(Environment.NewLine, diff));
        }
    }

    private static List<string> ScanDiff(SyntaxNode actual, SyntaxNode expected)
    {
        var actualChildren = actual.ChildNodes().ToList();
        var expectedChildren = expected.ChildNodes().ToList();
        var failedStatements = new List<string>();
        for (var i = 0; i < actualChildren.Count; i++)
        {
            if (expectedChildren.Count <= i)
            {
                failedStatements.Add($"Extra statements: {actualChildren[i]}");
                continue;
            }
            if ((actualChildren[i] is not StatementSyntax) || (actualChildren[i] is BlockSyntax or IfStatementSyntax or ForStatementSyntax or WhileStatementSyntax ))
            {
                failedStatements.AddRange(ScanDiff(actualChildren[i], expectedChildren[i]));
                continue;
            }
            if (!actualChildren[i].IsEquivalentTo(expectedChildren[i]))
            {
                failedStatements.Add($"Not equivalent. Actual:{Environment.NewLine}{actualChildren[i]}{Environment.NewLine}Expected:{Environment.NewLine}{expectedChildren[i]}");
            }
        }
        return failedStatements;
    }

    public static void ShouldBeWithNewlineReplace(this string actual, string expected)
    {
        var replaced = expected.Replace("\r\n", Environment.NewLine, StringComparison.InvariantCultureIgnoreCase);
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
