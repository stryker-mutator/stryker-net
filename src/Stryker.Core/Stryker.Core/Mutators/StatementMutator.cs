using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators;

public class StatementMutator : MutatorBase<StatementSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Standard;

    private static readonly HashSet<SyntaxKind> AllowedSyntaxes = new HashSet<SyntaxKind>()
    {
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
    };

    public override IEnumerable<Mutation> ApplyMutations(StatementSyntax node, SemanticModel semanticModel)
    {
        if (!AllowedSyntaxes.Contains(node.Kind()))
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

        // flux-control inside switch-case may cause a compile error
        if ((node is ReturnStatementSyntax ||
             node is BreakStatementSyntax ||
             node is ContinueStatementSyntax ||
             node is GotoStatementSyntax ||
             node is ThrowStatementSyntax) &&
            node.Ancestors().OfType<SwitchSectionSyntax>().Any())
        {
            yield break;
        }

        if (node is ExpressionStatementSyntax expressionNode)
        {
            // removing an assignment may cause a compile error
            if (expressionNode
                .DescendantNodes(s => !(s is AnonymousFunctionExpressionSyntax))
                .OfType<AssignmentExpressionSyntax>().Any())
            {
                yield break;
            }

            // removing an out variable may cause a compile error
            if (expressionNode
                .DescendantTokens(s => !(s is AnonymousFunctionExpressionSyntax))
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
}
