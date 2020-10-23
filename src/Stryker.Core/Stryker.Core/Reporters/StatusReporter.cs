using System;
using System.Collections.Generic;
using System.IO;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.Reporters
{
    public class StatusReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly TextWriter _consoleWriter;

        public StatusReporter(StrykerOptions strykerOptions, TextWriter consoleWriter = null)
        {
            _options = strykerOptions;
            _consoleWriter = consoleWriter ?? Console.Out;
        }
        public void OnAllMutantsTested(IReadOnlyInputComponent reportComponent)
        {
            // This reporter does not report during the testrun
        }

        public void OnMutantsCreated(IReadOnlyInputComponent reportComponent)
        {
            // This reporter does not report during the testrun
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
            // This reporter does not report during the testrun
        }

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested, IEnumerable<TestDescription> testDescriptions)
        {
            throw new System.NotImplementedException();
        }
    }
}