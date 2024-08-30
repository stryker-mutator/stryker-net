using System.Collections.Generic;
using Stryker.Core.Mutators;

namespace Stryker.Core.Mutants;

public interface ICSharpMutantOrchestrator
{
    IEnumerable<IMutator> Mutators { get; }
}
