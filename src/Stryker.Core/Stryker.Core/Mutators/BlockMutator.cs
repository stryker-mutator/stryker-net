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
