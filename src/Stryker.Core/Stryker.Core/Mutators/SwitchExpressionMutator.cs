using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    public class SwitchExpressionMutator : PatternMutatorBase<SwitchExpressionSyntax>
    {
        protected override Mutator Mutator => Mutator.SwitchExpression;

        public override IEnumerable<Mutation> ApplyMutations(SwitchExpressionSyntax node) => node
            .DescendantNodes()
            .OfType<PatternSyntax>()
            .SelectMany(x => ApplyMutations(x));
    }
}
