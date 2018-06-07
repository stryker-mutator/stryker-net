using Microsoft.CodeAnalysis;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public interface IMutator
    {
        IEnumerable<Mutation> Mutate(SyntaxNode node);
    }
}
