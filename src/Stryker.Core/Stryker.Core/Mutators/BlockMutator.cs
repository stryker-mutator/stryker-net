using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    class BlockMutator : MutatorBase<BlockSyntax>, IMutator
    {
        private const string MutationName = "Block removal mutation";

        public override MutationLevel MutationLevel => MutationLevel.Basic;

        public override IEnumerable<Mutation> ApplyMutations(BlockSyntax node)
        {
            if (node.IsEmpty() ||
                IsInfiniteWhileLoop(node) ||
                (IsInStructConstructor(node) && ContainsAssignments(node)))
            {
                yield break;
            }


            yield return new Mutation()
            {
                OriginalNode = node,
                ReplacementNode = SyntaxFactory.Block(),
                DisplayName = MutationName,
                Type = Mutator.Block
            };
        }

        private static bool IsInfiniteWhileLoop(BlockSyntax node) =>
            node.Parent is WhileStatementSyntax { Condition: LiteralExpressionSyntax cond } &&
            cond.Kind() == SyntaxKind.TrueLiteralExpression;

        private static bool ContainsAssignments(BlockSyntax node) => node
            .ChildNodes()
            .OfType<ExpressionStatementSyntax>()
            .Any(expressionSyntax => expressionSyntax.Expression is AssignmentExpressionSyntax);

        private static bool IsInStructConstructor(BlockSyntax node)
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
                        return true;
                }
            }

            return false;
        }
    }
}
