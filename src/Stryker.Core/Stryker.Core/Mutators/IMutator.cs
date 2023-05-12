namespace Stryker.Core.Mutators;
using Microsoft.CodeAnalysis;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using System.Collections.Generic;

public interface IMutator
{
    IEnumerable<Mutation> Mutate(SyntaxNode node, StrykerOptions options);
}
