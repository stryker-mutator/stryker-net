using Microsoft.Extensions.Logging;
using Stryker.Core.Compiling;
using Stryker.Core.CoverageAnalysis;
using Stryker.Core.Logging;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using Stryker.Core.ToolHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Stryker.Core.MutationTest
{
    public interface IMutationTestProcessProvider
    {
        IMutationTestProcess Provide(MutationTestInput mutationTestInput, IReporter reporter, IMutationTestExecutor mutationTestExecutor, IStrykerOptions options);
    }

    public class MutationTestProcessProvider : IMutationTestProcessProvider
    {
        public IMutationTestProcess Provide(MutationTestInput mutationTestInput,
            IReporter reporter,
            IMutationTestExecutor mutationTestExecutor,
            IStrykerOptions options)
        {
            return new MutationTestProcess(mutationTestInput, reporter, mutationTestExecutor, options: options);
        }
    }

    public interface IMutationTestProcess
    {
        MutationTestInput Input { get; }
        void Mutate();
        StrykerRunResult Test(IEnumerable<Mutant> mutantsToTest);
        void GetCoverage();
        void FilterMutants();
    }

    public class MutationTestProcess : IMutationTestProcess
    {
        public MutationTestInput Input { get; }
        private readonly ICompilingProcess _compilingProcess;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly IMutantFilter _mutantFilter;
        private readonly IMutationTestExecutor _mutationTestExecutor;
        private readonly IMutantOrchestrator _orchestrator;
        private readonly IReporter _reporter;
        private readonly ICoverageAnalyser _coverageAnalyser;
        private readonly IStrykerOptions _options;

        public MutationTestProcess(MutationTestInput mutationTestInput,
            IReporter reporter,
            IMutationTestExecutor mutationTestExecutor,
            IMutantOrchestrator orchestrator = null,
            ICompilingProcess compilingProcess = null,
            IFileSystem fileSystem = null,
            IMutantFilter mutantFilter = null,
            ICoverageAnalyser coverageAnalyser = null,
            IStrykerOptions options = null)
        {
            Input = mutationTestInput;
            _reporter = reporter;
            _options = options;
            _mutationTestExecutor = mutationTestExecutor;
            _orchestrator = orchestrator ?? new MutantOrchestrator(options: _options);
            _compilingProcess = compilingProcess ?? new CompilingProcess(mutationTestInput, new RollbackProcess());
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
            _coverageAnalyser = coverageAnalyser ?? new CoverageAnalyser(_options, _mutationTestExecutor, mutationTestInput);

            _mutantFilter = mutantFilter
                ?? MutantFilterFactory
                .Create(options);
        }

        public void Mutate()
        {
            // Mutate source files
            foreach (var file in Input.ProjectInfo.ProjectContents.GetAllFiles())
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

            _logger.LogDebug("{0} mutants created", Input.ProjectInfo.ProjectContents.Mutants.Count());

            CompileMutations();
        }

        private void CompileMutations()
        {
            using var ms = new MemoryStream();
            using var msForSymbols = _options.DevMode ? new MemoryStream() : null;
            // compile the mutated syntax trees
            var compileResult = _compilingProcess.Compile(Input.ProjectInfo.ProjectContents.CompilationSyntaxTrees, ms, msForSymbols, _options.DevMode);

            foreach (var testProject in Input.ProjectInfo.TestProjectAnalyzerResults)
            {
                var injectionPath = Input.ProjectInfo.GetInjectionPath(testProject);
                if (!_fileSystem.Directory.Exists(Path.GetDirectoryName(injectionPath)))
                {
                    _fileSystem.Directory.CreateDirectory(injectionPath);
                }

                // inject the mutated Assembly into the test project
                using var fs = _fileSystem.File.Create(Path.Combine(injectionPath, injectionPath));
                ms.Position = 0;
                ms.CopyTo(fs);

                if (msForSymbols != null)
                {
                    // inject the debug symbols into the test project
                    using var symbolDestination = _fileSystem.File.Create(Path.Combine(
                        Path.GetDirectoryName(injectionPath),
                        Input.ProjectInfo.ProjectUnderTestAnalyzerResult.GetSymbolFileName()));
                    msForSymbols.Position = 0;
                    msForSymbols.CopyTo(symbolDestination);
                }

                _logger.LogDebug("Injected the mutated assembly file into {0}", injectionPath);
            }

            // if a rollback took place, mark the rolled back mutants as status:BuildError
            if (compileResult.RollbackResult?.RollbackedIds.Any() ?? false)
            {
                foreach (var mutant in Input.ProjectInfo.ProjectContents.Mutants
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

            _logger.LogInformation("{0} mutants detected in {1}", Input.ProjectInfo.ProjectContents.TotalMutants.Count(), Input.ProjectInfo.ProjectContents.Name);
        }

        public StrykerRunResult Test(IEnumerable<Mutant> mutantsToTest)
        {
            if (!mutantsToTest.Any())
            {
                return new StrykerRunResult(_options, double.NaN);
            }
            var mutantGroups = BuildMutantGroupsForTest(mutantsToTest.ToList());

            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = _options.ConcurrentTestrunners };
            Parallel.ForEach(mutantGroups, parallelOptions, mutants =>
            {
                var testMutants = new HashSet<Mutant>();

                TestUpdateHandler testUpdateHandler = (testedMutants, failedTests, ranTests, timedOutTest) =>
                {
                    var mustGoOn = !_options.Optimizations.HasFlag(OptimizationFlags.AbortTestOnKill);
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
                };
                _mutationTestExecutor.Test(mutants, Input.TimeoutMs, testUpdateHandler);
                    
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

            _mutationTestExecutor.TestRunner.Dispose();

            return new StrykerRunResult(_options, Input.ProjectInfo.ProjectContents.GetMutationScore());
        }

        private IEnumerable<List<Mutant>> BuildMutantGroupsForTest(IReadOnlyCollection<Mutant> mutantsNotRun)
        {

            if (_options.Optimizations.HasFlag(OptimizationFlags.DisableTestMix) || !_options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest))
            {
                return mutantsNotRun.Select(x => new List<Mutant> { x });
            }

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

            _logger.LogDebug($"Mutations will be tested in {blocks.Count} test runs, instead of {mutantsNotRun.Count}.");
            return blocks;
        }

        public void GetCoverage()
        {
            _coverageAnalyser.DetermineTestCoverage();
        }

        public void FilterMutants()
        {
            foreach (var file in Input.ProjectInfo.ProjectContents.GetAllFiles())
            {
                _mutantFilter.FilterMutants(file.Mutants, file, _options);
            }

            var skippedMutants = Input.ProjectInfo.ProjectContents.ReadOnlyMutants.Where(m => m.ResultStatus != MutantStatus.NotRun);
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

            var notRunMutantsWithResultStatusReason = Input.ProjectInfo.ProjectContents.ReadOnlyMutants
                .Where(m => m.ResultStatus == MutantStatus.NotRun && !string.IsNullOrEmpty(m.ResultStatusReason))
                .GroupBy(x => x.ResultStatusReason);

            foreach (var notRunMutantReason in notRunMutantsWithResultStatusReason)
            {
                _logger.LogInformation(
                    LeftPadAndFormatForMutantCount(notRunMutantReason.Count(), "mutants will be tested because: {1}"),
                    notRunMutantReason.Count(),
                    notRunMutantReason.Key);
            }

            var notRunCount = Input.ProjectInfo.ProjectContents.ReadOnlyMutants.Count(m => m.ResultStatus == MutantStatus.NotRun);
            _logger.LogInformation(LeftPadAndFormatForMutantCount(notRunCount, "total mutants will be tested"), notRunCount);

            _reporter.OnMutantsCreated(Input.ProjectInfo.ProjectContents);
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