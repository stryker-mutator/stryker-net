using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;

namespace Stryker.Core.Reporters
{
    public interface IReporter
    {
        void OnInitialisationStarted();
        void OnInitialBuildStarted();
        void OnInitialTestRunStarted();
        void OnInitialisationDone();

        // Will get called when the project has been mutated
        void OnMutantsCreated(IReadOnlyInputComponent rapportComponent);
        // Will get called when a mutation has been tested
        void OnMutantTested(IReadOnlyMutant result);
        // Wille get callen when all mutations have been tested
        void OnAllMutantsTested(IReadOnlyInputComponent rapportComponent);
    }
}
