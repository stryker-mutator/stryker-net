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
        private readonly IBaselineProvider _baselineProvider;
        private readonly IGitInfoProvider _gitInfoProvider;

        private readonly StrykerOptions _options;
        private readonly JsonReport _baseline;
        private readonly ILogger<DiffMutantFilter> _logger;

        public string DisplayName => "git diff file filter";

        public DiffMutantFilter(StrykerOptions options, IDiffProvider diffProvider = null, IBaselineProvider baselineProvider = null, IGitInfoProvider gitInfoProvider = null)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DiffMutantFilter>();

            _options = options;
            _gitInfoProvider = gitInfoProvider ?? new GitInfoProvider(options);
            _baselineProvider = baselineProvider ?? BaselineProviderFactory.Create(options);

            if (options.CompareToDashboard)
            {
                _baseline = GetBaselineAsync().Result;
            }

            _diffResult = diffProvider.ScanDiff();

            if (_diffResult != null)
            {
                _logger.LogInformation("{0} files changed", _diffResult.ChangedFiles.Count);

                foreach (var changedFile in _diffResult.ChangedFiles)
                {
                    _logger.LogInformation("Changed file {0}", changedFile);
                }
            }
        }


        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions options)
        {
            // Mutants can be enabled for testing based on multiple reasons. We store all the filtered mutants in this list and return this list.
            var filteredMutants = new List<Mutant>();

            if (options.CompareToDashboard)
            {
                // If the dashboard feature is enabled but we cannot find a baseline. We are going to test the entire project. Thus none of the mutants can be filtered out and all are returned.
                if (_baseline == null)
                {
                    _logger.LogDebug("Testing all mutants on {0} because there is no baseline available", file.RelativePathToProjectFile);
                    return mutants;
                }

                // Updates all the mutants iun this file with their counterpart's result in the report of the previous run
                UpdateMutantsWithBaselineStatus(mutants, file);
            }

            // If the diff result flags this file as modified, we want to run all mutants again
            if (_diffResult.ChangedFiles.Contains(file.FullPath))
            {
                _logger.LogDebug("Returning all mutants in {0} because the file is modified", file.RelativePathToProjectFile);
                return SetMutantStatusForFileChanged(mutants);
            }


            // If any of the tests have been changed, we want to return all mutants covered by these testfiles.
            if (_diffResult.TestFilesChanged != null && _diffResult.TestFilesChanged.Any())
            {
                filteredMutants = ResetMutantStatusForChangedTests(mutants).ToList();
            }

            // Identical mutants within the same file cannot be distinguished from eachother and therefore we cannot give them a mutant status from the baseline. These ill have to be rerun.
            if (_options.CompareToDashboard)
            {
                var mutantsNotRun = GetMutantsWithStatusNotRun(mutants).ToList();
                filteredMutants = MergeMutantLists(filteredMutants, mutantsNotRun);
            }

            return filteredMutants;
        }


        private void UpdateMutantsWithBaselineStatus(IEnumerable<Mutant> mutants, FileLeaf file)
        {
            foreach (var baselineFile in _baseline.Files)
            {
                var filePath = FilePathUtils.NormalizePathSeparators(baselineFile.Key);

                if (filePath == file.RelativePath)
                {
                    foreach (var baselineMutant in baselineFile.Value.Mutants)
                    {
                        var baselineMutantSourceCode = GetMutantSourceCode(baselineFile.Value.Source, baselineMutant);

                        IEnumerable<Mutant> matchingMutants = GetMutantMatchingSourceCode(mutants, baselineMutant, baselineMutantSourceCode);

                        SetMutantStatusToBaselineMutantStatus(baselineMutant, matchingMutants);
                    }
                }
            }

            var mutantGroups = mutants
                .GroupBy(x => x.ResultStatusReason)
                .OrderBy(x => x.Key);

            foreach (var skippedMutantGroup in mutantGroups)
            {
                _logger.LogInformation("{0} mutants got status {1}. Reason: {2}", skippedMutantGroup.Count(),
                    skippedMutantGroup.First().ResultStatus, skippedMutantGroup.Key);
            }
        }

        private static void SetMutantStatusToBaselineMutantStatus(JsonMutant baselineMutant, IEnumerable<Mutant> matchingMutants)
        {
            if (matchingMutants.Count() == 1)
            {
                matchingMutants.First().ResultStatus = (MutantStatus)Enum.Parse(typeof(MutantStatus), baselineMutant.Status);
                matchingMutants.First().ResultStatusReason = "Result based on previous run.";
            }
            else
            {
                foreach (var matchingMutant in matchingMutants)
                {
                    matchingMutant.ResultStatus = MutantStatus.NotRun;
                    matchingMutant.ResultStatusReason = "Could not determine the correct mutant status";
                }
            }
        }

        public string GetMutantSourceCode(string source, JsonMutant baselineMutant)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(source);

            var beginLinePosition = new LinePosition(baselineMutant.Location.Start.Line - 1, baselineMutant.Location.Start.Column - 1);
            var endLinePosition = new LinePosition(baselineMutant.Location.End.Line - 1, baselineMutant.Location.End.Column - 1);

            LinePositionSpan span = new LinePositionSpan(beginLinePosition, endLinePosition);

            var textSpan = tree.GetText().Lines.GetTextSpan(span);

            return tree.GetRoot().DescendantNodes(textSpan)
                .First(n => textSpan.Equals(n.Span)).ToString();
        }

        private IEnumerable<Mutant> GetMutantMatchingSourceCode(IEnumerable<Mutant> mutants, JsonMutant baselineMutant, string baselineMutantSourceCode)
        {
            return mutants.Where(x =>
                x.Mutation.OriginalNode.ToString() == baselineMutantSourceCode &&
                x.Mutation.DisplayName == baselineMutant.MutatorName);
        }

        private async Task<JsonReport> GetBaselineAsync()
        {
            var branchName = _gitInfoProvider.GetCurrentBranchName();

            var baselineLocation = $"dashboard-compare/{branchName}";

            var report = await _baselineProvider.Load(baselineLocation);

            if (report == null)
            {
                _logger.LogInformation("We could not locate a baseline for branch {0}, now trying fallback version {1}", branchName, _options.FallbackVersion);

                return await GetFallbackBaselineAsync();
            }

            _logger.LogInformation("Found baseline report for current branch {0}", branchName);

            return report;
        }

        private async Task<JsonReport> GetFallbackBaselineAsync()
        {
            var report = await _baselineProvider.Load(_options.FallbackVersion);

            if (report == null)
            {
                _logger.LogInformation("We could not locate a baseline report for the current branch or fallback version. Now running a complete test to establish a baseline.");
                return null;
            }

            _logger.LogInformation("Found fallback report using version {0}", _options.FallbackVersion);

            return report;
        }

        private IEnumerable<Mutant> SetMutantStatusForFileChanged(IEnumerable<Mutant> mutants)
        {
            foreach (var mutant in mutants)
            {
                mutant.ResultStatus = MutantStatus.NotRun;
                mutant.ResultStatusReason = "File changed since last commit.";
            }

            return mutants;
        }

        private IEnumerable<Mutant> GetMutantsWithStatusNotRun(IEnumerable<Mutant> mutants)
        {
            return mutants.Where(x => x.ResultStatus == MutantStatus.NotRun);
        }

        private IEnumerable<Mutant> ResetMutantStatusForChangedTests(IEnumerable<Mutant> mutants)
        {
            var filteredMutants = new List<Mutant>();

            foreach (var mutant in mutants)
            {
                var coveringTests = mutant.CoveringTests.Tests;

                if (coveringTests.Any(coveringTest => _diffResult.TestFilesChanged.Any(changedTestFile => coveringTest.TestfilePath == changedTestFile))
                    || coveringTests.Any(coveringTest => coveringTest.IsAllTests))
                {
                    mutant.ResultStatus = MutantStatus.NotRun;
                    mutant.ResultStatusReason = "One or more covering tests changed";

                    filteredMutants.Add(mutant);
                    break;
                }
            }

            return filteredMutants;
        }

        /// Takes two lists. Adds the mutants from the updateMutants list to the targetMutants. 
        /// If the targetMutants already contain a member with the same Id. The results are updated.
        private List<Mutant> MergeMutantLists(List<Mutant> targetMutants, List<Mutant> updateMutants)
        {
            foreach (var (updateMutant, targetMutant) in from updateMutant in updateMutants
                                                         let targetMutant = targetMutants.FirstOrDefault(filtered => filtered.Id == updateMutant.Id)
                                                         select (updateMutant, targetMutant))
            {
                if (targetMutant != null)
                {
                    targetMutant.ResultStatus = updateMutant.ResultStatus;
                    targetMutant.ResultStatusReason = updateMutant.ResultStatusReason;
                }
                else
                {
                    targetMutants.Add(updateMutant);
                }
            }

            return targetMutants;
        }
    }
}