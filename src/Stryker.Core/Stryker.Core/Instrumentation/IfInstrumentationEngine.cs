using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Instrumentation;

/// <summary>
/// Injects a mutation controlled by an if Statement.
/// </summary>
internal class IfInstrumentationEngine : BaseEngine<IfStatementSyntax>
{
    private readonly SyntaxAnnotation Blockmarker = new SyntaxAnnotation("Blocked");
    /// <summary>
    /// Injects an if statement with the original code or the mutated one, depending on condition's result.
    /// </summary>
    /// <param name="condition">Expression for the condition.</param>
    /// <param name="originalNode">Original code</param>
    /// <param name="mutatedNode">Mutated code</param>
    /// <returns>A statement containing the expected construct.</returns>
    /// <remarks>This method works with statement and block.</remarks>
    public IfStatementSyntax InjectIf(ExpressionSyntax condition, StatementSyntax originalNode, StatementSyntax mutatedNode)
    {
        var block = AsBlock(originalNode);
        return SyntaxFactory.IfStatement(condition,
                AsBlock(mutatedNode),
                SyntaxFactory.ElseClause(block.WithoutTrivia()))
            .WithTriviaFrom(block)
            .WithAdditionalAnnotations(Marker);
    }

    private BlockSyntax AsBlock(StatementSyntax code)
    {
        if (code is not BlockSyntax block)
        {
            // we create a single statement block and surface the trivia to the block
            return SyntaxFactory.Block(code.WithoutTrivia()).WithTriviaFrom(code).WithAdditionalAnnotations(Blockmarker);
        }

        return block.HasSignificantTrivia() ?
            // we need to wrap the block if it has a single statement with trivia
            SyntaxFactory.Block(block).WithAdditionalAnnotations(Blockmarker) : block;
    }

    private StatementSyntax RemoveBlockIfNeeded(StatementSyntax code)
    {
        if (code.HasAnnotation(Blockmarker) && code is BlockSyntax { Statements.Count: 1 } block)
        {
            return block.Statements[0].WithTriviaFrom(code);
        }
        return code;
    }

    /// <summary>
    /// Returns the original code.
    /// </summary>
    /// <param name="ifNode">if statement to be 'removed'</param>
    /// <returns>the original node.</returns>
    /// <remarks>this method returns either a single statement or a syntax block.</remarks>
    protected override SyntaxNode Revert(IfStatementSyntax ifNode)
    {
        if (ifNode.Else?.Statement is BlockSyntax block)
        {
            return RemoveBlockIfNeeded(block).WithTriviaFrom(ifNode);
        }

        throw new InvalidOperationException(
            $"Expected a block containing an 'else' statement, found:\n{ifNode.ToFullString()}.");
    }

    protected override bool Erases(IfStatementSyntax node, Func<SyntaxNode, bool> predicate) =>
        // check if identifier is assigned on false condition and not assigned on true
        !node.Statement.ContainsNodeThatVerifies(predicate, false)
        && node.Else.ContainsNodeThatVerifies(predicate, false);
}
