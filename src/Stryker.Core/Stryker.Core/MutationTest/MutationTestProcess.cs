using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Compiling;
using Stryker.Core.DiffProviders;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Logging;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Reporters;

namespace Stryker.Core.MutationTest
{
    public interface IMutationTestProcess
    {
        void Mutate();
        StrykerRunResult Test(StrykerOptions options);
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
            string repoPath = Repository.Discover(options.BasePath)?.Split(".git")[0];
            if (string.IsNullOrEmpty(repoPath))
            {
                _logger.LogWarning("Could not locate git repo. Unable to determine git diff to filter mutants.");
                return;
            }
            using (var repo = new Repository(repoPath))
            {
                _mutantFilters = mutantFilters ?? new IMutantFilter[]
                {
                    new FilePatternMutantFilter(),
                    new IgnoredMethodMutantFilter(),
                    new ExcludeMutationMutantFilter(),
                    new DiffMutantFilter(options, new GitDiffProvider(options, repo, repoPath))
                };
            }
        }

        public void Mutate()
        {
            _logger.LogDebug("Injecting helpers into assembly.");
            var mutatedSyntaxTrees = new List<SyntaxTree>();
            var cSharpParseOptions = new CSharpParseOptions(_options.LanguageVersion);
            foreach (var helper in CodeInjection.MutantHelpers)
            {
                mutatedSyntaxTrees.Add(CSharpSyntaxTree.ParseText(helper.Value, path: helper.Key,
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

                // Filter the mutants
                var allMutants = _orchestrator.GetLatestMutantBatch();
                IEnumerable<Mutant> filteredMutants = allMutants;

                foreach (var mutantFilter in _mutantFilters)
                {
                    var current = mutantFilter.FilterMutants(filteredMutants, file, _options).ToList();

                    // Mark the filtered mutants as skipped
                    foreach (var skippedMutant in filteredMutants.Except(current))
                    {
                        skippedMutant.ResultStatus = MutantStatus.Skipped;
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

                var injectionPath = _input.ProjectInfo.GetInjectionPath();
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

                // if a rollback took place, mark the rollbacked mutants as status:BuildError
                if (compileResult.RollbackResult?.RollbackedIds.Any() ?? false)
                {
                    foreach (var mutant in _input.ProjectInfo.ProjectContents.Mutants
                        .Where(x => compileResult.RollbackResult.RollbackedIds.Contains(x.Id)))
                    {
                        // Ignore compilation errors if the mutation is skipped anyways.
                        if (mutant.ResultStatus == MutantStatus.Skipped)
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
                if (_input.ProjectInfo.ProjectContents.Mutants.All(x => x.ResultStatus == MutantStatus.Skipped))
                {
                    _logger.LogWarning("It looks like all mutants were excluded, try a re-run with less exclusion.");
                }
                if (_input.ProjectInfo.ProjectContents.Mutants.Any(x => x.ResultStatus == MutantStatus.NoCoverage))
                {
                    _logger.LogWarning("Not a single mutant is covered by a test. Go add some tests!");
                }
                if (!_input.ProjectInfo.ProjectContents.Mutants.Any())
                {
                    _logger.LogWarning("It\'s a mutant-free world, nothing to test.");
                    return new StrykerRunResult(options, null);
                }
            }

            var mutantsToTest = mutantsNotRun.Where(x => x.ResultStatus != MutantStatus.Skipped && x.ResultStatus != MutantStatus.NoCoverage);
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
    }
}