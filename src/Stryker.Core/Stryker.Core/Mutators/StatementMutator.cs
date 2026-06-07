using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions;

namespace Stryker.Core.Mutators;

public class StatementMutator : MutatorBase<StatementSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Standard;

    private static readonly HashSet<SyntaxKind> _allowedSyntaxes =
    [
        // SyntaxKind.EmptyStatement, // useless mutation

        // SyntaxKind.Block, // unitary mutation should prevail over blocks
        // SyntaxKind.CheckedStatement, // not unitary
        // SyntaxKind.UncheckedStatement, // not unitary
        // SyntaxKind.FixedStatement, // not unitary
        // SyntaxKind.UnsafeStatement, // not unitary
        // SyntaxKind.UsingStatement, // not unitary
        // SyntaxKind.LockStatement, // not unitary

        // SyntaxKind.LabeledStatement, // error: definition
        // SyntaxKind.LocalDeclarationStatement, // error: definition
        // SyntaxKind.LocalFunctionStatement, // error: definition

        // SyntaxKind.IfStatement, // mutated in other mutators, not unitary
        // SyntaxKind.WhileStatement, // mutated in other mutators, not unitary
        // SyntaxKind.DoStatement, // mutated in other mutators, not unitary
        // SyntaxKind.ForStatement, // mutated in other mutators, not unitary
        // SyntaxKind.ForEachStatement, // not unitary
        // SyntaxKind.ForEachVariableStatement, // not unitary
        // SyntaxKind.TryStatement, // not unitary
        // SyntaxKind.SwitchStatement, // not unitary
            
        SyntaxKind.ReturnStatement,
        SyntaxKind.BreakStatement,
        SyntaxKind.ContinueStatement,
        SyntaxKind.GotoStatement,
        SyntaxKind.ThrowStatement,
        SyntaxKind.YieldReturnStatement,
        SyntaxKind.YieldBreakStatement,

        SyntaxKind.ExpressionStatement,
    ];

    public override IEnumerable<Mutation> ApplyMutations(StatementSyntax node, SemanticModel semanticModel)
    {
        if (!_allowedSyntaxes.Contains(node.Kind()))
        {
            yield break;
        }

        if (node is ReturnStatementSyntax returnNode)
        {
            // non-void return statements may cause a compile error
            if (returnNode.Expression != null)
            {
                yield break;
            }

            // return inside ifs with a declaration may cause a compile error
            if (returnNode.FirstAncestorOrSelf<IfStatementSyntax>()?.Condition.DescendantNodes().OfType<DeclarationPatternSyntax>().Any() ?? false)
            {
                yield break;
            }
        }

        // flow-control inside switch-case may cause a compile error
        if ((node is ReturnStatementSyntax ||
             node is BreakStatementSyntax ||
             node is ContinueStatementSyntax ||
             node is GotoStatementSyntax ||
             node is ThrowStatementSyntax) &&
            node.Ancestors().OfType<SwitchSectionSyntax>().Any())
        {
            yield break;
        }

        // a throw that is the only top-level terminating statement in a getter body
        // would leave the getter with no valid code path, causing CS0161
        // (for non-void methods CS0161 is handled by the rollback process)
        if (node is ThrowStatementSyntax && IsOnlyControlFlowInGetterRequiringReturn(node))
        {
            yield break;
        }

        if (node is ExpressionStatementSyntax expressionNode)
        {
            // removing an assignment may cause a compile error
            if (expressionNode
                .DescendantNodes(s => s is not AnonymousFunctionExpressionSyntax)
                .OfType<AssignmentExpressionSyntax>().Any())
            {
                yield break;
            }

            // removing an out variable may cause a compile error
            if (expressionNode
                .DescendantTokens(s => s is not AnonymousFunctionExpressionSyntax)
                .Any(t => t.IsKind(SyntaxKind.OutKeyword)))
            {
                yield break;
            }
        }

        yield return new Mutation
        {
            OriginalNode = node,
            ReplacementNode = SyntaxFactory.EmptyStatement(),
            DisplayName = @"Statement mutation",
            Type = Mutator.Statement
        };
    }

    /// <summary>
    /// Returns true when the throw is the only top-level terminating statement in a property getter body.
    /// Replacing it with an empty statement would cause <see href="https://learn.microsoft.com/dotnet/csharp/misc/cs0161">CS0161</see>.
    /// For non-void methods the same error is handled by the rollback process.
    /// </summary>
    private static bool IsOnlyControlFlowInGetterRequiringReturn(StatementSyntax node)
    {
        if (node.Parent is BlockSyntax parentBlock)
        {
            // Block must itself be the direct body of a get accessor.
            return parentBlock.Parent is AccessorDeclarationSyntax { RawKind: (int)SyntaxKind.GetAccessorDeclaration }
                && !parentBlock.Statements.Any(s => s != node && IsReturnLike(s));
        }

        return false;
    }

    private static bool IsReturnLike(StatementSyntax s) =>
        s.IsKind(SyntaxKind.ReturnStatement) || s.IsKind(SyntaxKind.ThrowStatement);
}
