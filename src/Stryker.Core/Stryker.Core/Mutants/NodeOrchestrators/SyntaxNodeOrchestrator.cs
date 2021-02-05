﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class SyntaxNodeOrchestrator: NodeSpecificOrchestrator<SyntaxNode, SyntaxNode>
    {
        // we don't mutate this node
        protected override IEnumerable<Mutant> GenerateMutationForNode(SyntaxNode node, MutationContext context)
        {
            return Enumerable.Empty<Mutant>();
        }

        public SyntaxNodeOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
