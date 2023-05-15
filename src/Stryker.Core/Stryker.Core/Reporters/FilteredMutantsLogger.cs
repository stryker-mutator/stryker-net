using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.Reporters
{
    public class FilteredMutantsLogger
    {
        private readonly ILogger<FilteredMutantsLogger> _logger;

        public FilteredMutantsLogger(ILogger<FilteredMutantsLogger> logger = null)
        {
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<FilteredMutantsLogger>();
        }

        public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent)
        {
            var skippedMutants = reportComponent.Mutants.Where(m => m.ResultStatus != MutantStatus.Pending);

            var skippedMutantGroups = skippedMutants.GroupBy(x => new { x.ResultStatus, x.ResultStatusReason }).OrderBy(x => x.Key.ResultStatusReason);

            foreach (var skippedMutantGroup in skippedMutantGroups)
            {
                _logger.LogInformation(
                    FormatStatusReasonLogString(skippedMutantGroup.Count(), skippedMutantGroup.Key.ResultStatus),
                    skippedMutantGroup.Count(), skippedMutantGroup.Key.ResultStatus, skippedMutantGroup.Key.ResultStatusReason);
            }

            if (skippedMutants.Any())
            {
                _logger.LogInformation(
                    LeftPadAndFormatForMutantCount(skippedMutants.Count(), "total mutants are skipped for the above mentioned reasons"),
                    skippedMutants.Count());
            }

            var notRunMutantsWithResultStatusReason = reportComponent.Mutants
                .Where(m => m.ResultStatus == MutantStatus.Pending && !string.IsNullOrEmpty(m.ResultStatusReason))
                .GroupBy(x => x.ResultStatusReason);

            foreach (var notRunMutantReason in notRunMutantsWithResultStatusReason)
            {
                _logger.LogInformation(
                    LeftPadAndFormatForMutantCount(notRunMutantReason.Count(), "mutants will be tested because: {1}"),
                    notRunMutantReason.Count(),
                    notRunMutantReason.Key);
            }

            var notRunCount = reportComponent.Mutants.Count(m => m.ResultStatus == MutantStatus.Pending);
            _logger.LogInformation(LeftPadAndFormatForMutantCount(notRunCount, "total mutants will be tested"), notRunCount);
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
