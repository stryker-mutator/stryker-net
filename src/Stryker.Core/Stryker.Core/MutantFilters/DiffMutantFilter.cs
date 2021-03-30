using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using Stryker.Core.Baseline;
using Stryker.Core.DashboardCompare;
using Stryker.Core.DiffProviders;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stryker.Core.MutantFilters
{
    public class DiffMutantFilter : IMutantFilter
    {
        private readonly DiffResult _diffResult;
        private readonly ILogger<DiffMutantFilter> _logger;

        public string DisplayName => "git diff file filter";

        public DiffMutantFilter(IDiffProvider diffProvider = null)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DiffMutantFilter>();

            _diffResult = diffProvider.ScanDiff();

            if (_diffResult != null)
            {
                _logger.LogInformation("{0} files changed", _diffResult.ChangedSourceFiles?.Count ?? 0 + _diffResult.ChangedTestFiles?.Count ?? 0);

                if (_diffResult.ChangedSourceFiles != null)
                {
                    foreach (var changedFile in _diffResult.ChangedSourceFiles)
                    {
                        _logger.LogInformation("Changed file {0}", changedFile);
                    }
                }
                if (_diffResult.ChangedTestFiles != null)
                {
                    foreach (var changedFile in _diffResult.ChangedTestFiles)
                    {
                        _logger.LogInformation("Changed test file {0}", changedFile);
                    }
                }
            }
        }

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, ReadOnlyFileLeaf file, IStrykerOptions options)
        {
            // Mutants can be enabled for testing based on multiple reasons. We store all the filtered mutants in this list and return this list.
            IEnumerable<Mutant> filteredMutants;

            // A non-csharp file is flagged by the diff result as modified. We cannot determine which mutants will be affected by this, thus all mutants have to be tested.
            if (_diffResult.ChangedTestFiles is { } && _diffResult.ChangedTestFiles.Any(x => !x.EndsWith(".cs")))
            {
                _logger.LogDebug("Returning all mutants in {0} because a non-source file is modified", file.RelativePath);
                return SetMutantStatusForNonCSharpFileChanged(mutants);
            }

            // If the diff result flags this file as modified, we want to run all mutants again
            if (_diffResult.ChangedSourceFiles != null && _diffResult.ChangedSourceFiles.Contains(file.FullPath))
            {
                _logger.LogDebug("Returning all mutants in {0} because the file is modified", file.RelativePath);
                return SetMutantStatusForFileChanged(mutants);
            }
            else
            {
                filteredMutants = SetNotRunMutantsToIgnored(mutants);
            }

            // If any of the tests have been changed, we want to return all mutants covered by these testfiles.
            // Only check for changed c# files. Other files have already been handled.
            if (_diffResult.ChangedTestFiles != null && _diffResult.ChangedTestFiles.Any(file => file.EndsWith(".cs")))
            {
                filteredMutants = ResetMutantStatusForChangedTests(mutants);
            }

            return filteredMutants;
        }

        private IEnumerable<Mutant> SetNotRunMutantsToIgnored(IEnumerable<Mutant> mutants)
        {
            foreach (var mutant in mutants.Where(m => m.ResultStatus == MutantStatus.NotRun))
            {
                mutant.ResultStatus = MutantStatus.Ignored;
                mutant.ResultStatusReason = "Mutant not changed compared to target commit";
            }

            return new List<Mutant>();
        }

        private IEnumerable<Mutant> SetMutantStatusForFileChanged(IEnumerable<Mutant> mutants)
        {
            foreach (var mutant in mutants)
            {
                mutant.ResultStatus = MutantStatus.NotRun;
                mutant.ResultStatusReason = "Mutant changed compared to target commit";
            }

            return mutants;
        }

        private IEnumerable<Mutant> SetMutantStatusForNonCSharpFileChanged(IEnumerable<Mutant> mutants)
        {
            foreach (var mutant in mutants)
            {
                mutant.ResultStatus = MutantStatus.NotRun;
                mutant.ResultStatusReason = "Non-CSharp files in test project were changed";
            }

            return mutants;
        }

        private IEnumerable<Mutant> ResetMutantStatusForChangedTests(IEnumerable<Mutant> mutants)
        {
            var filteredMutants = new List<Mutant>();

            foreach (var mutant in mutants)
            {
                var coveringTests = mutant.CoveringTests.Tests;

                if (coveringTests != null
                    && (coveringTests.Any(coveringTest => _diffResult.ChangedTestFiles.Any(changedTestFile => coveringTest.TestFilePath == changedTestFile))))
                {
                    mutant.ResultStatus = MutantStatus.NotRun;
                    mutant.ResultStatusReason = "One or more covering tests changed";

                    filteredMutants.Add(mutant);
                }
            }

            return filteredMutants;
        }
    }
}
