﻿using Crayon;
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
        private readonly TextWriter _consoleWriter;

        public ConsoleDotProgressReporter(TextWriter consoleWriter = null)
        {
            _consoleWriter = consoleWriter ?? Console.Out;
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

        public void OnAllMutantsTested(IReadOnlyInputComponent inputComponent) { }
    }
}
