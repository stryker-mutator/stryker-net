using Crayon;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents;
using System;
using System.Collections.Generic;
using System.IO;

namespace Stryker.Core.Reporters
{
    /// <summary>
    /// The default reporter, prints a simple progress by printing dots
    /// </summary>
    public class ConsoleDotProgressReporter : IReporter
    {
        private readonly TextWriter _output;

        public ConsoleDotProgressReporter(TextWriter output = null)
        {
            _output = output ?? Console.Out;
        }

        public void OnMutantsCreated(IReadOnlyInputComponent inputComponent) { }

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> m, IEnumerable<TestDescription> t)
        {
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
            switch (result.ResultStatus)
            {
                case MutantStatus.Killed:
                    _output.Write(".");
                    break;
                case MutantStatus.Survived:
                    _output.Write(Output.Red("S"));
                    break;
                case MutantStatus.Timeout:
                    _output.Write("T");
                    break;
            };
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent inputComponent) { }
    }
}
