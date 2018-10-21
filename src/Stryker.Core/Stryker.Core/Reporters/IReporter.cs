﻿using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;

namespace Stryker.Core.Reporters
{
    public interface IReporter
    {
        // Will get called when the project has been mutated
        void OnMutantsCreated(IReadOnlyInputComponent reportComponent);
        // Will get called when a mutation has been tested
        void OnMutantTested(IReadOnlyMutant result);
        // Will get called when all mutations have been tested
        void OnAllMutantsTested(IReadOnlyInputComponent reportComponent);
    }
}
