using System;
using Crayon;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var notRunMutantsWithResultStatusReason = reportComponent.ReadOnlyMutants
                .Where(m => m.ResultStatus == MutantStatus.NotRun && !string.IsNullOrEmpty(m.ResultStatusReason))
                .GroupBy(x => x.ResultStatusReason);

            foreach (var notRunMutantReason in notRunMutantsWithResultStatusReason)
            {
                _consoleWriter.WriteLine(LeftPadAndFormatForMutantCount(notRunMutantReason.Count(), "mutants will be tested because: {1}"));
            }

            var notRunCount = reportComponent.ReadOnlyMutants.Count(m => m.ResultStatus == MutantStatus.NotRun);
            _consoleWriter.WriteLine(LeftPadAndFormatForMutantCount(notRunCount, "total mutants will be tested"));
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
            // This reporter does not report during the testrun
        }

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested, IEnumerable<TestDescription> testDescriptions)
        {
            var skippedMutantGroups = mutantsToBeTested.GroupBy(x => new { x.ResultStatus, x.ResultStatusReason }).OrderBy(x => x.Key.ResultStatusReason);

            foreach (var skippedMutantGroup in skippedMutantGroups)
            {
                _consoleWriter.WriteLine(FormatStatusReasonLogString(skippedMutantGroup.Count(), skippedMutantGroup.Key.ResultStatus));
            }

            if (mutantsToBeTested.Any())
            {
                _consoleWriter.WriteLine(LeftPadAndFormatForMutantCount(mutantsToBeTested.Count(), "total mutants are skipped for the above mentioned reasons"));
            }

        }

        private string FormatStatusReasonLogString(int mutantCount, MutantStatus resultStatus)
        {
            // Pad for status CompileError length
            var padForResultStatusLength = 13 - resultStatus.ToString().Length;

            var formattedString = LeftPadAndFormatForMutantCount(mutantCount, "mutants got status {1}.");
            formattedString += "Reason: {2}".PadLeft(11 + padForResultStatusLength);

            return formattedString;
        }

        private string LeftPadAndFormatForMutantCount(int mutantCount, string logString)
        {
            // Pad for max 5 digits mutant amount
            var padLengthForMutantCount = 5 - mutantCount.ToString().Length;
            return "{0} " + logString.PadLeft(logString.Length + padLengthForMutantCount);
        }
    }
}