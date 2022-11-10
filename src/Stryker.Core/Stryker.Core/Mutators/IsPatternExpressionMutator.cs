using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    public class IsPatternExpressionMutator : PatternMutatorBase<IsPatternExpressionSyntax>
    {
        protected override Mutator Mutator => Mutator.IsPatternExpression;

        public override IEnumerable<Mutation> ApplyMutations(IsPatternExpressionSyntax node) => node
            .DescendantNodes()
            .OfType<PatternSyntax>()
            .SelectMany(x => ApplyMutations(x));
    }
}
