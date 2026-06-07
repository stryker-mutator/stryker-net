using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutators;

public class BlockMutator : MutatorBase<BlockSyntax>
{
    private const string MutationName = "Block removal mutation";

    public override MutationLevel MutationLevel => MutationLevel.Basic;

    public override IEnumerable<Mutation> ApplyMutations(BlockSyntax node, SemanticModel semanticModel)
    {
        if (node.IsEmpty() ||
            IsInfiniteWhileLoop(node) ||
            CantBeMutated(node) ||
            IsGetterBodyWithoutReturn(node))
        {
            yield break;
        }

        yield return new Mutation
        {
            OriginalNode = node,
            ReplacementNode = SyntaxFactory.Block().WithCleanTriviaFrom(node),
            DisplayName = MutationName,
            Type = Mutator.Block
        };
    }

    private static bool IsInfiniteWhileLoop(SyntaxNode node) =>
        node.Parent is WhileStatementSyntax { Condition: LiteralExpressionSyntax cond } &&
        cond.Kind() == SyntaxKind.TrueLiteralExpression;

    private static bool ContainsAssignments(SyntaxNode node) => node
        .ChildNodes()
        .OfType<ExpressionStatementSyntax>()
        .Any(expressionSyntax => expressionSyntax.Expression is AssignmentExpressionSyntax);

    private static bool CantBeMutated(SyntaxNode node)
    {
        foreach (var ancestor in node.Ancestors())
        {
            switch (ancestor)
            {
                // Don't count local functions as being part of the constructor.
                // They can't assign to members, so they can be mutated.
                case LocalFunctionStatementSyntax:
                    return false;
                case ConstructorDeclarationSyntax { Parent: StructDeclarationSyntax }:
                    return ContainsAssignments(node);
                case SwitchSectionSyntax:
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns true when the block is the direct body of a property getter and contains no return statements.
    /// Replacing it with an empty block would cause <see href="https://learn.microsoft.com/dotnet/csharp/misc/cs0161">CS0161</see>
    /// because the injected if-true branch would have no return value, and <see cref="EndingReturnEngine"/> skips
    /// adding <c>return default</c> when there are no return statements in the block.
    /// For methods, CS0161 is handled by the rollback process; for getters it falls to safe mode.
    /// </summary>
    private static bool IsGetterBodyWithoutReturn(BlockSyntax node) =>
        node.Parent is AccessorDeclarationSyntax { RawKind: (int)SyntaxKind.GetAccessorDeclaration }
        && !node.ScanChildStatements(s => s.IsKind(SyntaxKind.ReturnStatement));
}
