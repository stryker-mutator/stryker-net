using Microsoft.CodeAnalysis;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Mutants;

namespace Stryker.Abstractions.Mutators;

public interface IMutator
{
    IEnumerable<Mutation> Mutate(SyntaxNode node, SemanticModel semanticModel, IStrykerOptions options);
}
