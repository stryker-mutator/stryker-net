using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
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
            // Note: using local functions below so that the check order can be easily reordered,
            // if a need to optimize arises at some point.

            bool IsInStructConstructor() => node.Ancestors()
                .OfType<ConstructorDeclarationSyntax>()
                .Any(constructor => constructor.Parent is StructDeclarationSyntax);

            bool ContainsAssignments() => node
                .ChildNodes()
                .OfType<ExpressionStatementSyntax>()
                .Any(expressionSyntax => expressionSyntax.Expression is AssignmentExpressionSyntax);

            bool FirstReturnTypedAncestorReturnsNonVoid() => node
                    .Ancestors()
                    .FirstOrDefault(ancestor =>
                        ancestor is MethodDeclarationSyntax or LocalFunctionStatementSyntax) switch
                {
                    MethodDeclarationSyntax method => !method.ReturnType.IsVoid(),
                    LocalFunctionStatementSyntax localFunction => !localFunction.ReturnType.IsVoid(),
                    _ => false,
                };

            bool ContainsReturns() => node
                .ChildNodes()
                .OfType<ReturnStatementSyntax>()
                .Any();

            if (!node.ChildNodes().Any() || (IsInStructConstructor() && ContainsAssignments()))
            {
                yield break;
            }

            if (FirstReturnTypedAncestorReturnsNonVoid() && ContainsReturns())
            {
                yield return new Mutation()
                {
                    OriginalNode = node,
                    ReplacementNode = SyntaxFactory.Block(SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression))),
                    DisplayName = MutationName,
                    Type = Mutator.Block
                };
            }
            else
            {
                yield return new Mutation()
                {
                    OriginalNode = node,
                    ReplacementNode = SyntaxFactory.Block(),
                    DisplayName = MutationName,
                    Type = Mutator.Block
                };
            }
        }
    }
}
