﻿using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Compiling;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stryker.Core.MutantFilters;

namespace Stryker.Core.MutationTest
{
    public interface IMutationTestProcess
    {
        void Mutate(StrykerOptions options);
        StrykerRunResult Test(StrykerOptions options);
        void Optimize(TestCoverageInfos coveredMutants);
    }

    public class MutationTestProcess : IMutationTestProcess
    {
        private readonly MutationTestInput _input;
        private readonly IReporter _reporter;
        private readonly IMutantOrchestrator _orchestrator;
        private readonly IFileSystem _fileSystem;
        private readonly ICompilingProcess _compilingProcess;
        private readonly IMutationTestExecutor _mutationTestExecutor;
        private readonly IEnumerable<IMutantFilter> _mutantFilters;
        private readonly ILogger _logger;

        public MutationTestProcess(MutationTestInput mutationTestInput,
            IReporter reporter,
            IMutationTestExecutor mutationTestExecutor,
            IMutantOrchestrator orchestrator = null,
            ICompilingProcess compilingProcess = null,
            IFileSystem fileSystem = null,
            IEnumerable<IMutantFilter> mutantFilters = null)
        {
            _input = mutationTestInput;
            _reporter = reporter;
            _mutationTestExecutor = mutationTestExecutor;
            _mutantFilters = mutantFilters;
            _orchestrator = orchestrator ?? new MutantOrchestrator();
            _compilingProcess = compilingProcess ?? new CompilingProcess(mutationTestInput, new RollbackProcess());
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
            _mutantFilters = mutantFilters ?? new IMutantFilter[]
            {
                new IgnoredFileMutantFilter(),
                new IgnoredMethodMutantFilter(),
                new ExcludeMutationMutantFilter(),
            };
        }

        public void Mutate(StrykerOptions options)
        {
            _logger.LogDebug("Injecting helpers into assembly.");
            var mutatedSyntaxTrees = new List<SyntaxTree>();
            foreach (var helper in CodeInjection.MutantHelpers)
            {
                mutatedSyntaxTrees.Add(CSharpSyntaxTree.ParseText(helper.Value, path: helper.Key, options: new CSharpParseOptions(options.LanguageVersion)));
            }

            foreach (var file in _input.ProjectInfo.ProjectContents.GetAllFiles())
            {
                // Get the syntax tree for the source file
                var syntaxTree = CSharpSyntaxTree.ParseText(file.SourceCode,
                    path: file.FullPath,
                    options: new CSharpParseOptions(options.LanguageVersion));

                // Mutate the syntax tree
                var mutatedSyntaxTree = _orchestrator.Mutate(syntaxTree.GetRoot());
                // Add the mutated syntax tree for compilation
                mutatedSyntaxTrees.Add(mutatedSyntaxTree.SyntaxTree);

                // Filter the mutants
                var allMutants = _orchestrator.GetLatestMutantBatch();
                IEnumerable<Mutant> filteredMutants = allMutants;

                foreach (var mutantFilter in _mutantFilters)
                {
                    var current = mutantFilter.FilterMutants(filteredMutants, file, options).ToList();

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
                var compileResult = _compilingProcess.Compile(mutatedSyntaxTrees, ms, options.DevMode);

                string injectionPath = _input.ProjectInfo.GetInjectionPath();
                if (!_fileSystem.Directory.Exists(Path.GetDirectoryName(injectionPath)) && !_fileSystem.File.Exists(injectionPath))
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
                        mutant.ResultStatus = MutantStatus.CompileError;
                        mutant.ResultStatusReason = "Could not compile";
                    }
                }
            }

            var skippedMutantGroups = _input.ProjectInfo.ProjectContents.GetAllFiles()
                .SelectMany(f => f.Mutants)
                .Where(x => x.ResultStatus != MutantStatus.NotRun).GroupBy(x => (x.ResultStatus, x.ResultStatusReason)).OrderBy(x => x.Key.ResultStatus);

            foreach (var skippedMutantGroup in skippedMutantGroups)
            {
                _logger.LogInformation("{0} mutants got status {1}. Reason: {2}", skippedMutantGroup.Count(), skippedMutantGroup.Key.ResultStatus, skippedMutantGroup.Key.ResultStatusReason);
            }

            _logger.LogInformation("{0} mutants ready for test", _input.ProjectInfo.ProjectContents.TotalMutants.Count());

            _reporter.OnMutantsCreated(_input.ProjectInfo.ProjectContents);
        }

        public StrykerRunResult Test(StrykerOptions options)
        {
            var mutantsNotRun = _input.ProjectInfo.ProjectContents.Mutants.Where(x => x.ResultStatus == MutantStatus.NotRun).ToList();
            if (!mutantsNotRun.Any())
            {
                if (_input.ProjectInfo.ProjectContents.Mutants.Any(x => x.ResultStatus == MutantStatus.Skipped))
                {
                    _logger.LogWarning("It looks like all mutants were excluded, try a re-run with less exclusion.");
                }
                else
                {
                    _logger.LogWarning("It\'s a mutant-free world, nothing to test.");
                }
                return new StrykerRunResult(options, null);
            }

            _reporter.OnStartMutantTestRun(mutantsNotRun);

            Parallel.ForEach(
                mutantsNotRun,
                new ParallelOptions { MaxDegreeOfParallelism = options.ConcurrentTestrunners },
                mutant =>
                {
                    _mutationTestExecutor.Test(mutant, _input.TimeoutMs);

                    _reporter.OnMutantTested(mutant);
                });

            _reporter.OnAllMutantsTested(_input.ProjectInfo.ProjectContents);

            _mutationTestExecutor.TestRunner.Dispose();

            return new StrykerRunResult(options, _input.ProjectInfo.ProjectContents.GetMutationScore());
        }

        public void Optimize(TestCoverageInfos coveredMutants)
        {
            if (coveredMutants == null)
            {
                return;
            }
            var covered = new HashSet<int>(coveredMutants.CoveredMutants);
            if (covered.Count == 0)
            {
                _logger.LogDebug("No mutant is covered by any test, no optimization done.");
                return;
            }
            _logger.LogDebug("Optimize test runs according to coverage info.");
            var report = new StringBuilder();
            var nonTested = _input.ProjectInfo.ProjectContents.Mutants.Where(x =>
                x.ResultStatus == MutantStatus.NotRun && !covered.Contains(x.Id)).ToList();
            const MutantStatus mutantResultStatus = MutantStatus.Survived;
            foreach (var mutant in nonTested)
            {
                mutant.ResultStatus = mutantResultStatus;
            }

            foreach (var mutant in _input.ProjectInfo.ProjectContents.Mutants)
            {
                var tests = coveredMutants.GetTests<object>(mutant.Id);
                if (tests == null)
                {
                    continue;
                }
                mutant.CoveringTest = tests.Select(x => x.ToString()).ToList();
            }

            report.AppendJoin(',', nonTested.Select(x => x.Id));
            _logger.LogInformation(nonTested.Count == 0
                ? "Congratulations, all mutants are covered by tests!"
                : $"{nonTested.Count} mutants are not reached by any tests and will survive! (Marked as {mutantResultStatus}).");
        }
    }
}
