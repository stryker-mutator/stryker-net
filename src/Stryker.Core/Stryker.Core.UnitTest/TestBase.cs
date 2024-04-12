using Microsoft.Build.Logging.StructuredLogger;
using Microsoft.CodeAnalysis.CSharp;
using System;
using Microsoft.Extensions.Logging;
using Shouldly;
using Stryker.Core.Logging;
using Microsoft.CodeAnalysis;
using System.Linq;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Mutants;

namespace Stryker.Core.UnitTest;

public abstract class TestBase
{
    protected CodeInjection codeInjection;
    protected MutantPlacer placer;

    protected TestBase() =>
        // initialize loggerfactory to prevent exceptions
        ApplicationLogging.LoggerFactory = new LoggerFactory();

    protected void CheckMutantPlacerProperlyPlaceAndRemoveHelpers<T>(string sourceCode, string expectedCode,
        Func<T, T> placer, Predicate<T> condition = null) where T : SyntaxNode =>
        CheckMutantPlacerProperlyPlaceAndRemoveHelpers<T, T>(sourceCode, expectedCode, placer, condition);

    protected void CheckMutantPlacerProperlyPlaceAndRemoveHelpers<T, TU>(string sourceCode, string expectedCode,
        Func<T, T> placer, Predicate<T> condition = null) where T : SyntaxNode where TU : SyntaxNode
    {
        var actualNode = CSharpSyntaxTree.ParseText(sourceCode).GetRoot();

        var node = actualNode.DescendantNodes().First(t => t is T ct && (condition == null || condition(ct))) as T;
        // inject helper
        actualNode = actualNode.ReplaceNode(node, placer(node));
        actualNode.ToFullString().ShouldBeSemantically(expectedCode);

        TU newNode;
        if (typeof(TU) == typeof(T))
        {
            newNode = actualNode.DescendantNodes().First(t => t is TU && t.ContainsAnnotations) as TU;
        }
        else
        {
            newNode = actualNode.DescendantNodes().First(t => t is T).DescendantNodes().First(t => t is TU && t.ContainsAnnotations) as TU;
        }

        // Remove helper
        var restored = this.placer.RemoveMutant(newNode);
        actualNode = actualNode.ReplaceNode(newNode, restored);
        actualNode.ToFullString().ShouldBeSemantically(sourceCode);
        // try to remove again
        Should.Throw<InvalidOperationException>(() => this.placer.RemoveMutant(restored));
    }
}
