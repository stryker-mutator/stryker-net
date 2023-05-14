
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Stryker.Core.DiffProviders;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.MutantFilters
{
    public class SinceMutantFilter : IMutantFilter
    {
        private readonly DiffResult _diffResult;
        private readonly TestSet _tests;
        private readonly ILogger<SinceMutantFilter> _logger;

        public MutantFilter Type => MutantFilter.Since;
        public string DisplayName => "since filter";

        public SinceMutantFilter(IDiffProvider diffProvider = null)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<SinceMutantFilter>();

            _diffResult = diffProvider.ScanDiff();
            _tests = diffProvider.Tests;

            if (_diffResult != null)
            {
                _logger.LogInformation("{0} files changed", (_diffResult.ChangedSourceFiles?.Count ?? 0) + (_diffResult.ChangedTestFiles?.Count ?? 0));

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

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, IReadOnlyFileLeaf file, StrykerOptions options)
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
            foreach (var mutant in mutants.Where(m => m.ResultStatus == MutantStatus.Pending || m.ResultStatus == MutantStatus.NoCoverage))
            {
                mutant.ResultStatus = MutantStatus.Ignored;
                mutant.ResultStatusReason = "Mutant not changed compared to target commit";
            }

            return new List<Mutant>();
        }

        private IEnumerable<Mutant> SetMutantStatusForFileChanged(IEnumerable<Mutant> mutants)
        {
            foreach (var mutant in mutants.Where(m => m.ResultStatus != MutantStatus.NoCoverage))
            {
                mutant.ResultStatus = MutantStatus.Pending;
                mutant.ResultStatusReason = "Mutant changed compared to target commit";
            }

            return mutants;
        }

        private IEnumerable<Mutant> SetMutantStatusForNonCSharpFileChanged(IEnumerable<Mutant> mutants)
        {
            foreach (var mutant in mutants.Where(m => m.ResultStatus != MutantStatus.NoCoverage))
            {
                mutant.ResultStatus = MutantStatus.Pending;
                mutant.ResultStatusReason = "Non-CSharp files in test project were changed";
            }

            return mutants;
        }

        private IEnumerable<Mutant> ResetMutantStatusForChangedTests(IEnumerable<Mutant> mutants)
        {
            var filteredMutants = new List<Mutant>();

            foreach (var mutant in mutants)
            {
                var coveringTests = _tests.Extract(mutant.CoveringTests.GetGuids());

                if (coveringTests != null
                    && coveringTests.Any(coveringTest => _diffResult.ChangedTestFiles.Any(changedTestFile => coveringTest.TestFilePath == changedTestFile)))
                {
                    mutant.ResultStatus = MutantStatus.Pending;
                    mutant.ResultStatusReason = "One or more covering tests changed";

                    filteredMutants.Add(mutant);
                }
            }

            return filteredMutants;
        }
    }
}
