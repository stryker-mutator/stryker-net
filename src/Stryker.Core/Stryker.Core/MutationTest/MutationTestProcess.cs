using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Compiling;
using Stryker.Core.CoverageAnalysis;
using Stryker.Core.Logging;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Stryker.Core.MutationTest
{
    public interface IMutationTestProcess
    {
        void Mutate();
        StrykerRunResult Test(StrykerOptions options);
        void GetCoverage();

        void FilterMutants();
    }

    public class MutationTestProcess : IMutationTestProcess
    {
        private readonly ICompilingProcess _compilingProcess;
        private readonly IFileSystem _fileSystem;
        private readonly MutationTestInput _input;
        private readonly ProjectComponent<FileLeaf, SyntaxTree> _projectInfo;
        private readonly ILogger _logger;
        private readonly IMutantFilter _mutantFilter;
        private readonly IMutationTestExecutor _mutationTestExecutor;
        private readonly IMutantOrchestrator _orchestrator;
        private readonly IReporter _reporter;
        private readonly ICoverageAnalyser _coverageAnalyser;
        private readonly StrykerOptions _options;

        public MutationTestProcess(MutationTestInput mutationTestInput,
            IReporter reporter,
            IMutationTestExecutor mutationTestExecutor,
            IMutantOrchestrator orchestrator = null,
            ICompilingProcess compilingProcess = null,
            IFileSystem fileSystem = null,
            StrykerOptions options = null,
            IMutantFilter mutantFilter = null,
            ICoverageAnalyser coverageAnalyser = null)
        {
            _input = mutationTestInput;
            _projectInfo = (ProjectComponent<FileLeaf, SyntaxTree>)mutationTestInput.ProjectInfo.ProjectContents;
            _reporter = reporter;
            _options = options;
            _mutationTestExecutor = mutationTestExecutor;
            _orchestrator = orchestrator ?? new MutantOrchestrator(options: _options);
            _compilingProcess = compilingProcess ?? new CompilingProcess(mutationTestInput, new RollbackProcess());
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
            _coverageAnalyser = coverageAnalyser ?? new CoverageAnalyser(_options, _mutationTestExecutor, _input);

            _mutantFilter = mutantFilter
                ?? MutantFilterFactory
                .Create(options);
        }

        public void Mutate()
        {
            // Mutate source files
            foreach (var file in _projectInfo.GetAllFiles())
            {
                _logger.LogDebug($"Mutating {file.Name}");
                // Mutate the syntax tree
                var mutatedSyntaxTree = _orchestrator.Mutate(file.SyntaxTree.GetRoot());
                // Add the mutated syntax tree for compilation
                file.MutatedSyntaxTree = mutatedSyntaxTree.SyntaxTree;
                if (_options.DevMode)
                {
                    _logger.LogTrace($"Mutated {file.Name}:{Environment.NewLine}{mutatedSyntaxTree.ToFullString()}");
                }
                // Filter the mutants
                var allMutants = _orchestrator.GetLatestMutantBatch();
                file.Mutants = allMutants;
            }

            _logger.LogDebug("{0} mutants created", _projectInfo.Mutants.Count());

            CompileMutations();
        }

        private void CompileMutations()
        {
            using var ms = new MemoryStream();
            using var msForSymbols = _options.DevMode ? new MemoryStream() : null;
            // compile the mutated syntax trees
            var compileResult = _compilingProcess.Compile(_projectInfo.CompilationSyntaxTrees, ms, msForSymbols, _options.DevMode);

            foreach (var testProject in _input.ProjectInfo.TestProjectAnalyzerResults)
            {
                var injectionPath = testProject.TargetDirectory;
                if (!_fileSystem.Directory.Exists(injectionPath))
                {
                    _fileSystem.Directory.CreateDirectory(injectionPath);
                }

                // inject the mutated Assembly into the test project
                using var fs = _fileSystem.File.Create(Path.Combine(injectionPath, _input.ProjectInfo.ProjectUnderTestAnalyzerResult.TargetFileName));
                ms.Position = 0;
                ms.CopyTo(fs);

                if (msForSymbols != null)
                {
                    // inject the debug symbols into the test project
                    using var symbolDestination = _fileSystem.File.Create(Path.Combine(injectionPath,
                        _input.ProjectInfo.ProjectUnderTestAnalyzerResult.SymbolFileName));
                    msForSymbols.Position = 0;
                    msForSymbols.CopyTo(symbolDestination);
                }

                _logger.LogDebug("Injected the mutated assembly file into {0}", injectionPath);
            }

            // if a rollback took place, mark the rolled back mutants as status:BuildError
            if (compileResult.RollbackResult?.RollbackedIds.Any() ?? false)
            {
                foreach (var mutant in _projectInfo.Mutants
                    .Where(x => compileResult.RollbackResult.RollbackedIds.Contains(x.Id)))
                {
                    // Ignore compilation errors if the mutation is skipped anyways.
                    if (mutant.ResultStatus == MutantStatus.Ignored)
                    {
                        continue;
                    }

                    mutant.ResultStatus = MutantStatus.CompileError;
                    mutant.ResultStatusReason = "Mutant caused compile errors";
                }
            }
        }

        public StrykerRunResult Test(StrykerOptions options)
        {
            var viableMutantsCount = _projectInfo.Mutants.Count(x => x.CountForStats);
            var mutantsNotRun = _projectInfo.Mutants.Where(x => x.ResultStatus == MutantStatus.NotRun).ToList();

            if (!mutantsNotRun.Any())
            {
                if (_projectInfo.Mutants.Any(x => x.ResultStatus == MutantStatus.Ignored))
                {
                    _logger.LogWarning("It looks like all mutants with tests were excluded. Try a re-run with less exclusion!");
                }
                if (_projectInfo.Mutants.Any(x => x.ResultStatus == MutantStatus.NoCoverage))
                {
                    _logger.LogWarning("It looks like all non-excluded mutants are not covered by a test. Go add some tests!");
                }
                if (!_projectInfo.Mutants.Any())
                {
                    _logger.LogWarning("It\'s a mutant-free world, nothing to test.");
                    return new StrykerRunResult(options, double.NaN);
                }
            }

            var mutantsToTest = mutantsNotRun;
            if (_options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest))
            {
                var testCount = _mutationTestExecutor.TestRunner.DiscoverNumberOfTests();
                var toTest = mutantsNotRun.Sum(x => x.MustRunAgainstAllTests ? testCount : x.CoveringTests.Count);
                var total = testCount * viableMutantsCount;
                if (total > 0 && total != toTest)
                {
                    _logger.LogInformation($"Coverage analysis will reduce run time by discarding {(total - toTest) / (double)total:P1} of tests because they would not change results.");
                }
            }
            else if (_options.Optimizations.HasFlag(OptimizationFlags.SkipUncoveredMutants))
            {
                var total = viableMutantsCount;
                var toTest = mutantsToTest.Count();
                if (total > 0 && total != toTest)
                {
                    _logger.LogInformation($"Coverage analysis will reduce run time by discarding {(total - toTest) / (double)total:P1} of tests because they would not change results.");
                }
            }

            if (mutantsToTest.Any())
            {
                var mutantGroups = BuildMutantGroupsForTest(mutantsNotRun);

                _reporter.OnStartMutantTestRun(mutantsNotRun, _mutationTestExecutor.TestRunner.Tests);

                Parallel.ForEach(
                    mutantGroups,
                    new ParallelOptions { MaxDegreeOfParallelism = options.ConcurrentTestrunners },
                    mutants =>
                    {
                        var testMutants = new HashSet<Mutant>();
                        _mutationTestExecutor.Test(mutants, _input.TimeoutMs,
                            (testedMutants, failedTests, ranTests, timedOutTest) =>
                            {
                                var mustGoOn = !options.Optimizations.HasFlag(OptimizationFlags.AbortTestOnKill);
                                foreach (var mutant in testedMutants)
                                {
                                    mutant.AnalyzeTestRun(failedTests, ranTests, timedOutTest);
                                    if (mutant.ResultStatus == MutantStatus.NotRun)
                                    {
                                        mustGoOn = true;
                                    }
                                    else if (!testMutants.Contains(mutant))
                                    {
                                        testMutants.Add(mutant);
                                        _reporter.OnMutantTested(mutant);
                                    }
                                }

                                return mustGoOn;
                            });

                        foreach (var mutant in mutants)
                        {
                            if (mutant.ResultStatus == MutantStatus.NotRun)
                            {
                                _logger.LogWarning($"Mutation {mutant.Id} was not fully tested.");
                            }
                            else if (!testMutants.Contains(mutant))
                            {
                                _reporter.OnMutantTested(mutant);
                            }
                        }
                    });
            }

            _reporter.OnAllMutantsTested(_projectInfo);

            _mutationTestExecutor.TestRunner.Dispose();

            return new StrykerRunResult(options, _projectInfo.GetMutationScore());
        }

        private IEnumerable<List<Mutant>> BuildMutantGroupsForTest(IReadOnlyCollection<Mutant> mutantsNotRun)
        {

            if (_options.Optimizations.HasFlag(OptimizationFlags.DisableTestMix) || !_options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest))
            {
                return mutantsNotRun.Select(x => new List<Mutant> { x });
            }

            _logger.LogInformation("Analyze coverage info to test multiple mutants per session.");
            var blocks = new List<List<Mutant>>(mutantsNotRun.Count);
            var mutantsToGroup = mutantsNotRun.ToList();
            // we deal with mutants needing full testing first
            blocks.AddRange(mutantsToGroup.Where(m => m.MustRunAgainstAllTests).Select(m => new List<Mutant> { m }));
            mutantsToGroup.RemoveAll(m => m.MustRunAgainstAllTests);
            var testsCount = mutantsToGroup.SelectMany(m => m.CoveringTests.GetList()).Distinct().Count();
            mutantsToGroup = mutantsToGroup.OrderByDescending(m => m.CoveringTests.Count).ToList();
            for (var i = 0; i < mutantsToGroup.Count; i++)
            {
                var usedTests = mutantsToGroup[i].CoveringTests.GetList().ToList();
                var nextBlock = new List<Mutant> { mutantsToGroup[i] };
                for (var j = i + 1; j < mutantsToGroup.Count; j++)
                {
                    if (mutantsToGroup[j].CoveringTests.Count + usedTests.Count > testsCount ||
                        mutantsToGroup[j].CoveringTests.ContainsAny(usedTests))
                    {
                        continue;
                    }

                    nextBlock.Add(mutantsToGroup[j]);
                    usedTests.AddRange(mutantsToGroup[j].CoveringTests.GetList());
                    mutantsToGroup.RemoveAt(j--);
                }

                blocks.Add(nextBlock);
            }
            // compute number of tests that will be run
            _logger.LogInformation($"Mutations will be tested in {blocks.Count} test runs, instead of {mutantsNotRun.Count}.");
            return blocks;
        }

        public void GetCoverage()
        {
            _coverageAnalyser.DetermineTestCoverage();
        }

        public void FilterMutants()
        {
            foreach (var file in _projectInfo.GetAllFiles())
            {
                _mutantFilter.FilterMutants(file.Mutants, file, _options);
            }

            var skippedMutants = _projectInfo.ReadOnlyMutants.Where(m => m.ResultStatus != MutantStatus.NotRun);
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

            var notRunMutantsWithResultStatusReason = _projectInfo.ReadOnlyMutants
                .Where(m => m.ResultStatus == MutantStatus.NotRun && !string.IsNullOrEmpty(m.ResultStatusReason))
                .GroupBy(x => x.ResultStatusReason);

            foreach (var notRunMutantReason in notRunMutantsWithResultStatusReason)
            {
                _logger.LogInformation(
                    LeftPadAndFormatForMutantCount(notRunMutantReason.Count(), "mutants will be tested because: {1}"),
                    notRunMutantReason.Count(),
                    notRunMutantReason.Key);
            }

            var notRunCount = _projectInfo.ReadOnlyMutants.Count(m => m.ResultStatus == MutantStatus.NotRun);
            _logger.LogInformation(LeftPadAndFormatForMutantCount(notRunCount, "total mutants will be tested"), notRunCount);

            _reporter.OnMutantsCreated(_projectInfo);
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