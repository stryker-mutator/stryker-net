using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Collections;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using static FSharp.Compiler.SyntaxTree;

namespace Stryker.Core.Mutants
{
    public class FsharpMutantOrchestrator : BaseMutantOrchestrator<FSharpList<SynModuleOrNamespace>>
    {
        private readonly FsharpCoreOrchestrator _base;

        //the current implementation only has the orchestrators and does not keep track of mutants or mutators
        public FsharpMutantOrchestrator(IEnumerable<IMutator> mutators = null, StrykerOptions options = null) : base(options)
        {
            Mutants = new Collection<Mutant>();
            _base = new FsharpCoreOrchestrator();
        }

        public override FSharpList<SynModuleOrNamespace> Mutate(FSharpList<SynModuleOrNamespace> input)
        {
            return _base.Mutate(input);
        }
    }
}
