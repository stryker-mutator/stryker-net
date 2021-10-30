using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    class BlockMutator : MutatorBase<BlockSyntax>, IMutator
    {
        private const string MutationName = "Block removal mutation";

        public override MutationLevel MutationLevel => MutationLevel.Basic;

        public override IEnumerable<Mutation> ApplyMutations(BlockSyntax node)
        {
            if (node.Parent is MethodDeclarationSyntax methodDeclaration && methodDeclaration.ReturnType.ToString() != "void")
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
