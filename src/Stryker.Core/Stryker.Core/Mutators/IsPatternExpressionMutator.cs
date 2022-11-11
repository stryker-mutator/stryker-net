using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    /// <summary> Mutator implementation for is expression</summary>
    public class IsPatternExpressionMutator : PatternMutatorBase<IsPatternExpressionSyntax>
    {
        /// <summary> Apply mutations to all <see cref="PatternSyntax"/> inside an <see cref="IsPatternExpressionSyntax"/></summary>
        public override IEnumerable<Mutation> ApplyMutations(IsPatternExpressionSyntax node) => node
            .DescendantNodes()
            .OfType<PatternSyntax>()
            .SelectMany(x => ApplyMutations(x));
    }
}
