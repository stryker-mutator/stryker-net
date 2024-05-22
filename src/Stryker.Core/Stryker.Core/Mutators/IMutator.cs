using Microsoft.CodeAnalysis;
using Stryker.Core.Options;
using Stryker.Shared.Mutants;
using Stryker.Shared.Options;
using System.Collections.Generic;

namespace Stryker.Core.Mutators;

public interface IMutator
{
    IEnumerable<IMutation> Mutate(SyntaxNode node, SemanticModel semanticModel, IStrykerOptions options);
}
