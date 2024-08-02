using Microsoft.CodeAnalysis;
using Stryker.Configuration.Options;
using Stryker.Configuration.Mutants;

namespace Stryker.Configuration.Mutators;

public interface IMutator
{
    IEnumerable<Mutation> Mutate(SyntaxNode node, SemanticModel semanticModel, IStrykerOptions options);
}
