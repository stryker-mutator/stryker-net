using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Compiling;
using Stryker.Core.DiffProviders;
using Stryker.Core.Exceptions;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Logging;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
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
        TestCoverageInfos GetCoverage();
    }

    public class MutationTestProcess : IMutationTestProcess
    {
        private readonly ICompilingProcess _compilingProcess;
        private readonly IFileSystem _fileSystem;
        private readonly MutationTestInput _input;
        private readonly ILogger _logger;
        private readonly IEnumerable<IMutantFilter> _mutantFilters;
        private readonly IMutationTestExecutor _mutationTestExecutor;
        private readonly IMutantOrchestrator _orchestrator;
        private readonly IReporter _reporter;
        private readonly StrykerOptions _options;

        public MutationTestProcess(MutationTestInput mutationTestInput,
            IReporter reporter,
            IMutationTestExecutor mutationTestExecutor,
            IMutantOrchestrator orchestrator = null,
            ICompilingProcess compilingProcess = null,
            IFileSystem fileSystem = null,
            StrykerOptions options = null,
            IEnumerable<IMutantFilter> mutantFilters = null)
        {
            _input = mutationTestInput;
            _reporter = reporter;
            _options = options;
            _mutationTestExecutor = mutationTestExecutor;
            _orchestrator = orchestrator ?? new MutantOrchestrator(options: _options);
            _compilingProcess = compilingProcess ?? new CompilingProcess(mutationTestInput, new RollbackProcess());
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
            _mutantFilters = mutantFilters ?? new IMutantFilter[]
                {
                    new FilePatternMutantFilter(),
                    new IgnoredMethodMutantFilter(),
                    new ExcludeMutationMutantFilter(),
                    new DiffMutantFilter(_options, new GitDiffProvider(_options)),
                    new ExcludeFromCodeCoverageFilter()
                };
        }

        public void Mutate()
        {
            var preprocessorSymbols = _input.ProjectInfo.ProjectUnderTestAnalyzerResult.DefineConstants;

            _logger.LogDebug("Injecting helpers into assembly.");
            var mutatedSyntaxTrees = new List<SyntaxTree>();
            var cSharpParseOptions = new CSharpParseOptions(_options.LanguageVersion, DocumentationMode.None, preprocessorSymbols: preprocessorSymbols);
            foreach (var (name, code) in CodeInjection.MutantHelpers)
            {
                mutatedSyntaxTrees.Add(CSharpSyntaxTree.ParseText(code, path: name,
                    options: cSharpParseOptions));
            }

            foreach (var file in _input.ProjectInfo.ProjectContents.GetAllFiles())
            {
                // Get the syntax tree for the source file
                var syntaxTree = CSharpSyntaxTree.ParseText(file.SourceCode,
                    path: file.FullPath,
                    options: cSharpParseOptions);

                _logger.LogDebug($"Mutating {file.Name}");
                // Mutate the syntax tree
                var mutatedSyntaxTree = _orchestrator.Mutate(syntaxTree.GetRoot());
                // Add the mutated syntax tree for compilation
                mutatedSyntaxTrees.Add(mutatedSyntaxTree.SyntaxTree);
                if (_options.DevMode)
                {
                    _logger.LogTrace($"Mutated {file.Name}:{Environment.NewLine}{mutatedSyntaxTree.ToFullString()}");
                }
                // Filter the mutants
                var allMutants = _orchestrator.GetLatestMutantBatch();
                IEnumerable<Mutant> filteredMutants = allMutants;

                foreach (var mutantFilter in _mutantFilters)
                {
                    var current = mutantFilter.FilterMutants(filteredMutants, file, _options).ToList();

                    // Mark the filtered mutants as skipped
                    foreach (var skippedMutant in filteredMutants.Except(current))
                    {
                        skippedMutant.ResultStatus = MutantStatus.Ignored;
                        skippedMutant.ResultStatusReason = $"Removed by {mutantFilter.DisplayName}";
                    }

                    filteredMutants = current;
                }

                // Store the generated mutants in the file
                file.Mutants = allMutants;
            }

            _logger.LogDebug("{0} mutants created", _input.ProjectInfo.ProjectContents.Mutants.Count());

            using (var ms = new MemoryStream())
            {
                // compile the mutated syntax trees
                var compileResult = _compilingProcess.Compile(mutatedSyntaxTrees, ms, _options.DevMode);

                foreach (var testProject in _input.ProjectInfo.TestProjectAnalyzerResults)
                {
                    var injectionPath = _input.ProjectInfo.GetInjectionPath(testProject);
                    if (!_fileSystem.Directory.Exists(Path.GetDirectoryName(injectionPath)) &&
                        !_fileSystem.File.Exists(injectionPath))
                    {
                        _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(injectionPath));
                    }

                    // inject the mutated Assembly into the test project
                    using (var fs = _fileSystem.File.Create(injectionPath))
                    {
                        ms.Position = 0;
                        ms.CopyTo(fs);
                    }

                    _logger.LogDebug("Injected the mutated assembly file into {0}", injectionPath);
                }

                // if a rollback took place, mark the rollbacked mutants as status:BuildError
                if (compileResult.RollbackResult?.RollbackedIds.Any() ?? false)
                {
                    foreach (var mutant in _input.ProjectInfo.ProjectContents.Mutants
                        .Where(x => compileResult.RollbackResult.RollbackedIds.Contains(x.Id)))
                    {
                        // Ignore compilation errors if the mutation is skipped anyways.
                        if (mutant.ResultStatus == MutantStatus.Ignored)
                        {
                            continue;
                        }

                        mutant.ResultStatus = MutantStatus.CompileError;
                        mutant.ResultStatusReason = "Could not compile";
                    }
                }
            }

            var skippedMutantGroups = _input.ProjectInfo.ProjectContents.GetAllFiles()
                .SelectMany(f => f.Mutants)
                .Where(x => x.ResultStatus != MutantStatus.NotRun).GroupBy(x => x.ResultStatusReason)
                .OrderBy(x => x.Key);

            foreach (var skippedMutantGroup in skippedMutantGroups)
            {
                _logger.LogInformation("{0} mutants got status {1}. Reason: {2}", skippedMutantGroup.Count(),
                    skippedMutantGroup.First().ResultStatus, skippedMutantGroup.Key);
            }

            _logger.LogInformation("{0} mutants ready for test",
                _input.ProjectInfo.ProjectContents.TotalMutants.Count());

            _reporter.OnMutantsCreated(_input.ProjectInfo.ProjectContents);
        }

        public StrykerRunResult Test(StrykerOptions options)
        {
            var mutantsNotRun = _input.ProjectInfo.ProjectContents.Mutants.Where(x => x.ResultStatus == MutantStatus.NotRun).ToList();

            if (!mutantsNotRun.Any())
            {
                if (_input.ProjectInfo.ProjectContents.Mutants.Any(x => x.ResultStatus == MutantStatus.Ignored))
                {
                    _logger.LogWarning("It looks like all mutants with tests were excluded. Try a re-run with less exclusion!");
                }
                if (_input.ProjectInfo.ProjectContents.Mutants.Any(x => x.ResultStatus == MutantStatus.NoCoverage))
                {
                    _logger.LogWarning("It looks like all non-excluded mutants are not covered by a test. Go add some tests!");
                }
                if (!_input.ProjectInfo.ProjectContents.Mutants.Any())
                {
                    _logger.LogWarning("It\'s a mutant-free world, nothing to test.");
                    return new StrykerRunResult(options, null);
                }
            }

            var mutantsToTest = mutantsNotRun.Where(x => x.ResultStatus != MutantStatus.Ignored && x.ResultStatus != MutantStatus.NoCoverage);
            if (mutantsToTest.Any())
            {
                _reporter.OnStartMutantTestRun(mutantsNotRun, _mutationTestExecutor.TestRunner.Tests);

                Parallel.ForEach(
                    mutantsNotRun,
                    new ParallelOptions { MaxDegreeOfParallelism = options.ConcurrentTestrunners },
                    mutant =>
                    {
                        _mutationTestExecutor.Test(mutant, _input.TimeoutMs);

                        _reporter.OnMutantTested(mutant);
                    });
            }

            _reporter.OnAllMutantsTested(_input.ProjectInfo.ProjectContents);

            _mutationTestExecutor.TestRunner.Dispose();

            return new StrykerRunResult(options, _input.ProjectInfo.ProjectContents.GetMutationScore());
        }

        public TestCoverageInfos GetCoverage()
        {
            var (targetFrameworkDoesNotSupportAppDomain, targetFramewoekDoesNotSupportPipe) = _input.ProjectInfo.ProjectUnderTestAnalyzerResult.CompatibilityModes;
            var testResult = _mutationTestExecutor.TestRunner.CaptureCoverage(targetFramewoekDoesNotSupportPipe, targetFrameworkDoesNotSupportAppDomain);
            if (testResult.Success)
            {
                return _mutationTestExecutor.TestRunner.CoverageMutants;
            }
            _logger.LogWarning("Test run with no active mutation failed. Stryker failed to correctly generate the mutated assembly. Please report this issue on github with a logfile of this run.");
            throw new StrykerInputException("No active mutant testrun was not successful.", testResult.ResultMessage);

        }
    }
}