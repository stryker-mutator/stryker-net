using System;
using System.Collections.Generic;
using System.IO;
using Crayon;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.Reporters
{
    /// <summary>
    /// The default reporter, prints a simple progress by printing dots
    /// </summary>
    public class ConsoleDotProgressReporter : IReporter
    {
        private readonly TextWriter _consoleWriter;

        public ConsoleDotProgressReporter(TextWriter consoleWriter = null) => _consoleWriter = consoleWriter ?? Console.Out;

        public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent) { }

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested)
        {
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
            switch (result.ResultStatus)
            {
                case MutantStatus.Killed:
                    _consoleWriter.Write(".");
                    break;
                case MutantStatus.Survived:
                    _consoleWriter.Write(Output.Red("S"));
                    break;
                case MutantStatus.Timeout:
                    _consoleWriter.Write("T");
                    break;
            };
        }

        public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo) { }
    }
}
