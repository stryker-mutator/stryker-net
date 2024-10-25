using System.Collections.Generic;
using Stryker.Abstractions.Mutators;

namespace Stryker.Core.Mutants;

public interface ICSharpMutantOrchestrator
{
    IEnumerable<IMutator> Mutators { get; }
}
