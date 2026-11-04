using Microsoft.CodeAnalysis;
using Stryker.Abstractions.Options;
using System.Collections.Generic;

namespace Stryker.Abstractions;

public interface IMutator
{
    IEnumerable<CsharpMutation> Mutate(SyntaxNode node, SemanticModel semanticModel, IStrykerOptions options);
}
