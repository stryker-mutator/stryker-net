using Microsoft.CodeAnalysis;
using Stryker.Abstractions.Options;
using System.Collections.Generic;

namespace Stryker.Abstractions;

public interface IMutator
{
    IEnumerable<Mutation> Mutate(SyntaxNode node, SemanticModel semanticModel, IStrykerOptions options);
}
