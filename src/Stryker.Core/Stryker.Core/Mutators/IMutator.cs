using Microsoft.CodeAnalysis;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using System.Collections.Generic;

namespace Stryker.Core.Mutators;

public interface IMutator
{
    IEnumerable<Mutation> Mutate(SyntaxNode node, SemanticModel semanticModel, StrykerOptions options);
}
