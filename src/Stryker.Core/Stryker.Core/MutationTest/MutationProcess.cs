﻿using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Stryker.Core.Compiling;
using Stryker.Core.Logging;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Stryker.Core.MutationTest
{
    public class MutationProcess : IMutationProcess
    {

        private readonly ProjectComponent<SyntaxTree> _projectInfo;
        private readonly ILogger _logger;
        private readonly StrykerOptions _options;
        private readonly CompilingProcess _compilingProcess;
        private readonly IFileSystem _fileSystem;
        private readonly MutationTestInput _input;
        private readonly IMutantOrchestrator _orchestrator;

        private readonly IMutantFilter _mutantFilter;
        private readonly IReporter _reporter;

        public MutationProcess(MutationTestInput mutationTestInput,
            IMutantOrchestrator orchestrator = null,
            IFileSystem fileSystem = null,
            StrykerOptions options = null,

            IMutantFilter mutantFilter = null,
            IReporter reporter = null)
        {
            _input = mutationTestInput;
            _projectInfo = (ProjectComponent<SyntaxTree>)mutationTestInput.ProjectInfo.ProjectContents;
            _options = options;
            _orchestrator = orchestrator ?? new MutantOrchestrator(options: _options);
            _compilingProcess = new CompilingProcess(mutationTestInput, new RollbackProcess());
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();

            _mutantFilter = mutantFilter ?? MutantFilterFactory.Create(options);
            _reporter = reporter;
        }

        public void Mutate()
        {
            // Mutate source files
            foreach (var file in _projectInfo.GetAllFiles().Cast<FileLeaf>())
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
        public void FilterMutants()
        {
            foreach (var file in _projectInfo.GetAllFiles())
            {
                // CompileError is a final status and can not be changed during filtering.
                var mutantsToFilter = file.Mutants.Where(x => x.ResultStatus != MutantStatus.CompileError);
                _mutantFilter.FilterMutants(mutantsToFilter, ((FileLeaf)file).ToReadOnly(), _options);
            }

            var skippedMutants = _projectInfo.Mutants.Where(m => m.ResultStatus != MutantStatus.NotRun);
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

            var notRunMutantsWithResultStatusReason = _projectInfo.Mutants
                .Where(m => m.ResultStatus == MutantStatus.NotRun && !string.IsNullOrEmpty(m.ResultStatusReason))
                .GroupBy(x => x.ResultStatusReason);

            foreach (var notRunMutantReason in notRunMutantsWithResultStatusReason)
            {
                _logger.LogInformation(
                    LeftPadAndFormatForMutantCount(notRunMutantReason.Count(), "mutants will be tested because: {1}"),
                    notRunMutantReason.Count(),
                    notRunMutantReason.Key);
            }

            var notRunCount = _projectInfo.Mutants.Count(m => m.ResultStatus == MutantStatus.NotRun);
            _logger.LogInformation(LeftPadAndFormatForMutantCount(notRunCount, "total mutants will be tested"), notRunCount);

            _reporter.OnMutantsCreated(_projectInfo.ToReadOnlyInputComponent());
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
