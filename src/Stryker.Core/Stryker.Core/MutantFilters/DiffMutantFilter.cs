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
        private readonly IBranchProvider _branchProvider;

        private readonly StrykerOptions _options;

        private readonly JsonReport _baseline;

        private readonly ILogger<DiffMutantFilter> _logger;

        public string DisplayName => "git diff file filter";

        public DiffMutantFilter(StrykerOptions options = null, IDiffProvider diffProvider = null, IBaselineProvider baselineProvider = null, IBranchProvider branchProvider = null)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DiffMutantFilter>();

            _baselineProvider = baselineProvider ?? BaselineProviderFactory.Create(options);
            _branchProvider = branchProvider ?? new GitBranchProvider(options);
            _options = options;

            if (options.CompareToDashboard)
            {
                _baseline = GetBaseline().Result;
            }

            _diffResult = diffProvider.ScanDiff();
        }

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions options)
        {
            var filteredMutants = new List<Mutant>();
            if (options.CompareToDashboard)
            {
                if (_baseline == null)
                {
                    return mutants;
                }

                UpdateMutantsWithBaseline(mutants, file);
            }

            if (_options.CompareToDashboard)
            {
                filteredMutants = ResetStatusForStatusUnclear(mutants).ToList();
            }

            if (_diffResult.ChangedFiles.Contains(file.FullPath))
            {
                UpdateMutantStatusses(filteredMutants, SetMutantStatusForFileChanged(mutants));
            }

            if (_diffResult.TestFilesChanged != null && _diffResult.TestFilesChanged.Any())
            {
                UpdateMutantStatusses(filteredMutants, ResetMutantStatusForChangedTests(mutants));
            }


            return filteredMutants.ToList();
        }

        private static void UpdateMutantStatusses(List<Mutant> mutants, IEnumerable<Mutant> mergingMutants)
        {
            foreach (var (mutant, filteredMutant) in from mutant in mergingMutants
                                                     let filteredMutant = mutants.FirstOrDefault(filtered => filtered.Id == mutant.Id)
                                                     select (mutant, filteredMutant))
            {
                if (filteredMutant != null)
                {
                    filteredMutant.ResultStatus = mutant.ResultStatus;
                    filteredMutant.ResultStatusReason = mutant.ResultStatusReason;
                }
                else
                {
                    mutants.Add(mutant);
                }

            }
        }

        private static IEnumerable<Mutant> ResetStatusForStatusUnclear(IEnumerable<Mutant> mutants)
        {
            var uncheckedMutants = new List<Mutant>();
            foreach (var mutant in mutants)
            {
                if (mutant.ResultStatusReason == "Could not affirm the correct mutant status")
                {
                    uncheckedMutants.Add(mutant);
                }
            }

            return uncheckedMutants;
        }

        private static IEnumerable<Mutant> SetMutantStatusForFileChanged(IEnumerable<Mutant> mutants)
        {
            foreach (var mutant in mutants)
            {
                mutant.ResultStatus = MutantStatus.NotRun;
                mutant.ResultStatusReason = "File changed since last commit.";
                yield return mutant;
            }
        }

        private IEnumerable<Mutant> ResetMutantStatusForChangedTests(IEnumerable<Mutant> mutants)
        {
            var filteredMutants = new List<Mutant>();

            foreach (var mutant in mutants)
            {
                var mutantCoveringTests = mutant.CoveringTests.Tests;

                if (mutantCoveringTests.Any(
                    coveringTest => _diffResult.TestFilesChanged.Any(
                        changedTestFile => coveringTest.TestfilePath == changedTestFile))
                   || mutantCoveringTests.Any(coveringTest => coveringTest.IsAllTests))
                {

                    mutant.ResultStatus = MutantStatus.NotRun;
                    mutant.ResultStatusReason = "One or more covering tests changed";

                    filteredMutants.Add(mutant);
                    break;
                }
            }

            return filteredMutants;
        }

        public async Task<JsonReport> GetBaseline()
        {
            var branchName = _branchProvider.GetCurrentBranchCanonicalName();

            _options.CurrentBranchCanonicalName = branchName;

            JsonReport report = null;
            if (branchName != null)
            {
                report = await _baselineProvider.Load(branchName);
            }

            if (report == null)
            {
                _logger.LogInformation("We could not locate a baseline for project {0}, now trying fallback Version", _options.ProjectName);

                return await GetFallbackBaseline();
            }

            _logger.LogInformation("Found report of project {0} using version {1} ", _options.ProjectName, branchName);

            return report;
        }

        public async Task<JsonReport> GetFallbackBaseline()
        {
            var report = await _baselineProvider.Load(_options.FallbackVersion);

            if (report == null)
            {
                _logger.LogInformation("We could not locate a baseline for project using fallback version. Now running a complete test to establish a baseline.");
                return null;
            }
            else
            {
                _logger.LogInformation("Found report of project {0} using version {1}", _options.ProjectName, _options.FallbackVersion);

                return report;
            }
        }

        private void UpdateMutantsWithBaseline(IEnumerable<Mutant> mutants, FileLeaf file)
        {
            foreach (var baselineFile in _baseline.Files)
            {
                var filePath = FilePathUtils.NormalizePathSeparators(baselineFile.Key);

                if (filePath == file.RelativePath)
                {
                    foreach (var baselineMutant in baselineFile.Value.Mutants)
                    {
                        var baselineMutantSourceCode = GetMutantSourceCode(baselineFile.Value.Source, baselineMutant);

                        IEnumerable<Mutant> matchingMutants = GetMatchingMutants(mutants, baselineMutant, baselineMutantSourceCode);

                        if (matchingMutants.Count() == 1)
                        {
                            UpdateMutantStatusWithBaseline(baselineMutant, matchingMutants);
                        }
                        else
                        {
                            UpdateMutantStatusForNoCertainResult(matchingMutants);
                        }
                    }
                }
            }
        }

        private void UpdateMutantStatusWithBaseline(JsonMutant baselineMutant, IEnumerable<Mutant> matchingMutants)
        {
            matchingMutants.First().ResultStatus = (MutantStatus)Enum.Parse(typeof(MutantStatus), baselineMutant.Status);
            matchingMutants.First().ResultStatusReason = "Result based on previous run.";
        }

        private IEnumerable<Mutant> GetMatchingMutants(IEnumerable<Mutant> mutants, JsonMutant baselineMutant, string baselineMutantSourceCode)
        {
            return mutants.Where(
            x => x.Mutation.OriginalNode.ToString() == baselineMutantSourceCode
            && x.Mutation.DisplayName == baselineMutant.MutatorName);
        }

        private void UpdateMutantStatusForNoCertainResult(IEnumerable<Mutant> matchingMutants)
        {
            foreach (var matchingMutant in matchingMutants)
            {
                matchingMutant.ResultStatus = MutantStatus.NotRun;
                matchingMutant.ResultStatusReason = "Could not affirm the correct mutant status";
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
    }
}
